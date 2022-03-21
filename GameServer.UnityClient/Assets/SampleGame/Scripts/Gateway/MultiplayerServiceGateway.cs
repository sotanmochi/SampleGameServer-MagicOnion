using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using GameServer.UnityClient;
using GameServer.Shared.MessagePackObject;
using SampleGame.Domain.Network;

namespace SampleGame.Gateway
{
    public sealed class MultiplayerServiceGateway : IDisposable
    {
        // public event Action<PlayerPose> OnReceivePlayerPose;

        public event Action<JoinResult> OnJoin;
        public event Action<JoinResult> OnLeave;
        public event Action<JoinResult> OnUserJoin;
        public event Action<JoinResult> OnUserLeave;

        private bool _connected;
        private bool _joined;

        private readonly GameStreamingClient _streamingClient;
        private readonly ChannelBase _channel;

        public MultiplayerServiceGateway(GameStreamingClient streamingClient, string address = "http://localhost:5000")
        {
            _streamingClient = streamingClient;
            _channel = GrpcChannelx.ForAddress(address);
            Initialize();
        }

        public void Initialize()
        {
            _streamingClient.OnReceivePlayerPose += OnReceivePlayerPoseEventHandler;
            _streamingClient.OnJoin += OnJoinEventHandler;
            _streamingClient.OnLeave += OnLeaveEventHandler;
            _streamingClient.OnUserJoin += OnUserJoinEventHandler;
            _streamingClient.OnUserLeave += OnUserLeaveEventHandler;
        }

        public void Dispose()
        {
            _streamingClient.OnReceivePlayerPose -= OnReceivePlayerPoseEventHandler;
            _streamingClient.OnJoin -= OnJoinEventHandler;
            _streamingClient.OnLeave -= OnLeaveEventHandler;
            _streamingClient.OnUserJoin -= OnUserJoinEventHandler;
            _streamingClient.OnUserLeave -= OnUserLeaveEventHandler;
        }

        public async UniTask<bool> Connect()
        {
            // Multithreading. Run on ThreadPool after this switching. 
            await UniTask.SwitchToThreadPool();
            DebugLogger.Log($"<color=orange>[{nameof(MultiplayerServiceGateway)}] Start of Connect | Thread Id: {Thread.CurrentThread.ManagedThreadId}</color>");

            // Work on ThreadPool. 
            _connected = await _streamingClient.ConnectAsync(_channel);

            // Return to MainThread (same as `ObserveOnMainThread` in UniRx). 
            await UniTask.SwitchToMainThread();
            DebugLogger.Log($"<color=orange>[{nameof(MultiplayerServiceGateway)}] End of Connect | Thread Id: {Thread.CurrentThread.ManagedThreadId}</color>");

            return _connected;
        }

        public async UniTask Disconnect()
        {
            _connected = false;
            await _streamingClient.Leave();
            await _streamingClient.DisconnectAsync();
        }

        public async UniTask<bool> Join(string roomId, string username)
        {
            // DebugLogger.Log($"[{nameof(MultiplayerServiceGateway)}] Join | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            await _streamingClient.Join(roomId, username);
            await UniTask.WaitUntil(() => _joined); // ToDo: Cancellation
            return _joined;
        }

        public async UniTask Leave()
        {
            await _streamingClient.Leave();
        }

        private void OnReceivePlayerPoseEventHandler(PlayerPose pose)
        {

        }

        private void OnJoinEventHandler(JoinResponse joinResponse)
        {
            // DebugLogger.Log($"[{nameof(MultiplayerServiceGateway)}] OnJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            _joined = true;

            OnJoin?.Invoke(new JoinResult()
            {
                ClientId = joinResponse.ClientId,
                RoomId = joinResponse.RoomId,
                Username = joinResponse.Username,
            });
        }

        private void OnLeaveEventHandler(JoinResponse joinResponse)
        {
            // DebugLogger.Log($"[{nameof(MultiplayerServiceGateway)}] OnLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            _joined = false;

            OnLeave?.Invoke(new JoinResult()
            {
                ClientId = joinResponse.ClientId,
                RoomId = joinResponse.RoomId,
                Username = joinResponse.Username,
            });
        }

        private void OnUserJoinEventHandler(JoinResponse joinResponse)
        {
            // DebugLogger.Log($"[{nameof(MultiplayerServiceGateway)}] OnUserJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            OnUserJoin?.Invoke(new JoinResult()
            {
                ClientId = joinResponse.ClientId,
                RoomId = joinResponse.RoomId,
                Username = joinResponse.Username,
            });
        }

        private void OnUserLeaveEventHandler(JoinResponse joinResponse)
        {
            // DebugLogger.Log($"[{nameof(MultiplayerServiceGateway)}] OnUserLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            OnUserLeave?.Invoke(new JoinResult()
            {
                ClientId = joinResponse.ClientId,
                RoomId = joinResponse.RoomId,
                Username = joinResponse.Username,
            });
        }
    }
}