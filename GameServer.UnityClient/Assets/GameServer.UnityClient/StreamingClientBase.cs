using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Grpc.Core;
using MagicOnion;

namespace GameServer.UnityClient
{
    public abstract class StreamingClientBase<TStreamingHub, TReceiver> where TStreamingHub : IStreamingHub<TStreamingHub, TReceiver>
    {
        public bool Initialized => _initialized;
        public bool Connected => _connected;

        protected abstract Task ConnectClientAsync();

        protected TStreamingHub _streamingClient;
        protected ChannelBase _channel;
        protected CancellationTokenSource _shutdownCancellation;

        private bool _initialized = false;
        private bool _connected = false;

        private bool _autoReconnect = false;
        private bool _wantsToQuit = false;
        private Type _streamingClientType;

        public bool Initialize()
        {
            if (_initialized)
            {
                DebugLogger.Log($"<color=orange>[{_streamingClientType}] Initialize | This client has already been initialized.</color>");
                return false;
            }

            _shutdownCancellation = new CancellationTokenSource();
            Application.wantsToQuit += Application_WantsToQuit;

            _streamingClientType = GetType();
            _initialized = true;

            return true;
        }

        public async Task DisposeAsync()
        {
            if (!_initialized)
            {
                DebugLogger.Log($"<color=orange>[{_streamingClientType}] DisposeAsync | This client has already been disposed.</color>");
                return;
            }

            _initialized = false;

            _shutdownCancellation.Cancel();
            await _streamingClient.DisposeAsync();

            Application.wantsToQuit -= Application_WantsToQuit;
            if (_wantsToQuit) Application.Quit();

            DebugLogger.Log($"[{_streamingClientType}] Disposed");
        }

        public async Task<bool> ConnectAsync(ChannelBase channel)
        {
            _channel = channel;

            if (!_initialized) Initialize();

            try
            {
                DebugLogger.Log($"[{_streamingClientType}] Connecting to the server...");

                await ConnectClientAsync();
                RegisterDisconnectEvent();

                DebugLogger.Log($"[{_streamingClientType}] Connection is established.");
            }
            catch (Exception e)
            {
                DebugLogger.LogError(e);
                return false;
            }

            _connected = true;
            return true;
        }

        public async Task DisconnectAsync(bool autoReconnect = false)
        {
            if (!_connected)
            {
                DebugLogger.Log($"<color=orange>[{_streamingClientType}] DisconnectAsync | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[{_streamingClientType}] Disconnecting from the server...");

            _autoReconnect = autoReconnect;
            await _streamingClient.DisposeAsync();

            DebugLogger.Log($"[{_streamingClientType}] Disconnected.");
        }

        private async void RegisterDisconnectEvent()
        {
            try
            {
                await _streamingClient.WaitForDisconnect(); // you can wait disconnected event
                _connected = false;
            }
            catch (Exception e)
            {
                DebugLogger.LogError(e);
            }
            finally
            {
                // try-to-reconnect? logging event? close? etc...
                DebugLogger.Log($"[{_streamingClientType}] Disconnected from the server.");

                if (_streamingClient != null && _autoReconnect)
                {
                    await Task.Delay(2000); // there is no particular meaning
                    await ReconnectAsync();
                }
            }
        }

        private async Task ReconnectAsync()
        {
            DebugLogger.Log($"[{_streamingClientType}] Reconnecting to the server...");

            await ConnectClientAsync();
            RegisterDisconnectEvent();

            DebugLogger.Log($"[{_streamingClientType}] Reconnected.");
        }

        private bool Application_WantsToQuit()
        {
            _wantsToQuit = true;
            DisposeAsync();
            return false;
        }
    }
}