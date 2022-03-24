using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using GameServer.UnityClient;
using GameServer.Shared.MessagePackObject;
using SampleGame.Domain.Player;

namespace SampleGame.Gateway
{
    public sealed class MultiplayerServiceGateway : IMultiplayerServiceGateway
    {
        public event Action<PlayerPose> OnPlayerPoseReceive;
        public event Action<JoinResult> OnJoin;
        public event Action<JoinResult> OnLeave;
        public event Action<JoinResult> OnUserJoin;
        public event Action<JoinResult> OnUserLeave;

        private bool _initialized;
        private bool _connected;
        private bool _joined;

        private readonly GameStreamingClient _streamingClient;
        private readonly ChannelBase _channel;

        public MultiplayerServiceGateway(MultiplayerServiceConfiguration configuration)
        {
            _streamingClient = new GameStreamingClient();
            _channel = GrpcChannelx.ForAddress(configuration.Address);
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized)
            {
                DebugLogger.LogWarning($"[{nameof(ChatServiceGateway)}] Already initialized");
                return;
            }

            _streamingClient.OnReceivePlayerPose += OnPlayerPoseReceiveEventHandler;
            _streamingClient.OnJoin += OnJoinEventHandler;
            _streamingClient.OnLeave += OnLeaveEventHandler;
            _streamingClient.OnUserJoin += OnUserJoinEventHandler;
            _streamingClient.OnUserLeave += OnUserLeaveEventHandler;

            _streamingClient.Initialize();

            _initialized = true;
        }

        public async UniTask Dispose()
        {
            _initialized = false;

            _streamingClient.OnReceivePlayerPose -= OnPlayerPoseReceiveEventHandler;
            _streamingClient.OnJoin -= OnJoinEventHandler;
            _streamingClient.OnLeave -= OnLeaveEventHandler;
            _streamingClient.OnUserJoin -= OnUserJoinEventHandler;
            _streamingClient.OnUserLeave -= OnUserLeaveEventHandler;

            await _streamingClient.DisposeAsync();
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
            if (!_connected) { await Connect(); }
            await _streamingClient.Join(roomId, username);
            await UniTask.WaitUntil(() => _joined); // ToDo: Cancellation
            return _joined;
        }

        public async UniTask Leave()
        {
            await _streamingClient.Leave();
        }

        public void SendPlayerPose(PlayerPose value)
        {
            _streamingClient.SendPlayerPose(new PlayerPoseObject()
            {
                PlayerId = value.PlayerId,
                Position = value.Position,
                Rotation = value.Rotation,
            });
        }

        private void OnPlayerPoseReceiveEventHandler(PlayerPoseObject pose)
        {
            // DebugLogger.Log($"[{nameof(MultiplayerServiceGateway)}] OnReceivePlayerPoseEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            OnPlayerPoseReceive?.Invoke(new PlayerPose()
            {
                PlayerId = pose.PlayerId,
                Position = pose.Position,
                Rotation = pose.Rotation,
            });
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