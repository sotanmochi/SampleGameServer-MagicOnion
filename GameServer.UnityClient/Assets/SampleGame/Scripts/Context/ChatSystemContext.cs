using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using UniRx;
using GameServer.UnityClient;

namespace SampleGame.Context
{
    public sealed class ChatSystemContext
    {
        public IObservable<(string Username, string Message)> OnReceiveMessage => _receiveMessageSubject;
        private readonly Subject<(string Username, string Message)> _receiveMessageSubject = new Subject<(string Username, string Message)>();

        private readonly ChatStreamingClient _streamingClient;
        private readonly ChannelBase _channel;

        private bool _connected;
        private bool _joined;

        private string _username;

        public ChatSystemContext(string address = "http://localhost:5000")
        {
            _streamingClient = new ChatStreamingClient();
            _channel = GrpcChannelx.ForAddress(address);
        }

        public void Initialize()
        {
            _streamingClient.OnJoin += OnJoinEventHandler;
            _streamingClient.OnLeave += OnLeaveEventHandler;
            _streamingClient.OnUserJoin += OnUserJoinEventHandler;
            _streamingClient.OnUserLeave += OnUserLeaveEventHandler;
            _streamingClient.OnReceiveMessage += OnReceiveMessageEventHandler;           
            _streamingClient.Initialize();
        }

        public async UniTask Dispose()
        {
            _receiveMessageSubject.Dispose();
            _streamingClient.OnJoin -= OnJoinEventHandler;
            _streamingClient.OnLeave -= OnLeaveEventHandler;
            _streamingClient.OnUserJoin -= OnUserJoinEventHandler;
            _streamingClient.OnUserLeave -= OnUserLeaveEventHandler;
            _streamingClient.OnReceiveMessage -= OnReceiveMessageEventHandler;
            await _streamingClient.DisposeAsync();
        }

        public async UniTask<bool> Connect()
        {
            // DebugLogger.Log($"[ChatSystemContext] Connect | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _connected = await _streamingClient.ConnectAsync(_channel);
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
            _streamingClient.SendMessage(message);
        }

        private void OnReceiveMessageEventHandler((string username, string message) data)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnReceiveMessageEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _receiveMessageSubject.OnNext(data);
        }

        private void OnJoinEventHandler((string RoomId, string Username) data)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _joined = true;
            _username = data.Username;
            _receiveMessageSubject.OnNext(("", $"{_username} has been joined."));
        }

        private void OnLeaveEventHandler((string RoomId, string Username) data)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _joined = false;
            _receiveMessageSubject.OnNext(("", $"{_username} has been joined."));
        }

        private void OnUserJoinEventHandler((string RoomId, string Username) data)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnUserJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _receiveMessageSubject.OnNext(("", $"{data.Username} has been joined."));
        }

        private void OnUserLeaveEventHandler((string RoomId, string Username) data)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnUserLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _receiveMessageSubject.OnNext(("", $"{data.Username} has been left the room."));
        }
    }
}