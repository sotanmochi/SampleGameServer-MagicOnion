using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using GameServer.Shared.MessagePackObject;
using GameServer.UnityClient;
using SampleGame.Domain.Chat;
using SampleGame.Domain.Network;

namespace SampleGame.Gateway
{
    public sealed class ChatServiceGateway
    {
        public event Action<ChatMessage> OnReceiveMessage;

        public event Action<JoinResult> OnJoin;
        public event Action<JoinResult> OnLeave;
        public event Action<JoinResult> OnUserJoin;
        public event Action<JoinResult> OnUserLeave;

        private bool _initialized;
        private bool _connected;
        private bool _joined;

        private readonly ChatStreamingClient _streamingClient;
        private readonly ChannelBase _channel;

        public ChatServiceGateway(string address = "http://localhost:5000")
        {
            _streamingClient = new ChatStreamingClient();
            _channel = GrpcChannelx.ForAddress(address);
        }

        public void Initialize()
        {
            if (_initialized)
            {
                DebugLogger.LogWarning($"[{nameof(ChatServiceGateway)}] Already initialized");
                return;
            }

            _streamingClient.OnJoin += OnJoinEventHandler;
            _streamingClient.OnLeave += OnLeaveEventHandler;
            _streamingClient.OnUserJoin += OnUserJoinEventHandler;
            _streamingClient.OnUserLeave += OnUserLeaveEventHandler;
            _streamingClient.OnReceiveMessage += OnReceiveMessageEventHandler;

            _streamingClient.Initialize();

            _initialized = true;
        }

        public async UniTask Dispose()
        {
            _initialized = false;

            _streamingClient.OnJoin -= OnJoinEventHandler;
            _streamingClient.OnLeave -= OnLeaveEventHandler;
            _streamingClient.OnUserJoin -= OnUserJoinEventHandler;
            _streamingClient.OnUserLeave -= OnUserLeaveEventHandler;
            _streamingClient.OnReceiveMessage -= OnReceiveMessageEventHandler;

            await _streamingClient.DisposeAsync();
        }

        public async UniTask<bool> Connect()
        {
            // Multithreading. Run on ThreadPool after this switching. 
            await UniTask.SwitchToThreadPool();
            DebugLogger.Log($"<color=orange>[ChatSystemContext] Start of Connect | Thread Id: {Thread.CurrentThread.ManagedThreadId}</color>");

            // Work on ThreadPool. 
            _connected = await _streamingClient.ConnectAsync(_channel);

            // Return to MainThread (same as `ObserveOnMainThread` in UniRx). 
            await UniTask.SwitchToMainThread();
            DebugLogger.Log($"<color=orange>[ChatSystemContext] End of Connect | Thread Id: {Thread.CurrentThread.ManagedThreadId}</color>");

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
            // DebugLogger.Log($"[ChatSystemContext] Join | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            await _streamingClient.Join(roomId, username);
            await UniTask.WaitUntil(() => _joined); // ToDo: Cancellation
            return _joined;
        }

        public async UniTask Leave()
        {
            await _streamingClient.Leave();
        }

        public void SendMessage(string message)
        {
            DebugLogger.Log($"[ChatSystemContext] SendMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _streamingClient.SendMessage(message);
        }

        private void OnReceiveMessageEventHandler(MessageResponse messageResponse)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnReceiveMessageEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnReceiveMessage?.Invoke(new ChatMessage()
            {
                Username = messageResponse.Username,
                Message = messageResponse.Message,
            });
        }

        private void OnJoinEventHandler(JoinResponse joinResponse)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

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
            // DebugLogger.Log($"[ChatSystemContext] OnLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

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
            // DebugLogger.Log($"[ChatSystemContext] OnUserJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            OnUserJoin?.Invoke(new JoinResult()
            {
                ClientId = joinResponse.ClientId,
                RoomId = joinResponse.RoomId,
                Username = joinResponse.Username,
            });
        }

        private void OnUserLeaveEventHandler(JoinResponse joinResponse)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnUserLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            OnUserLeave?.Invoke(new JoinResult()
            {
                ClientId = joinResponse.ClientId,
                RoomId = joinResponse.RoomId,
                Username = joinResponse.Username,
            });
        }
    }
}