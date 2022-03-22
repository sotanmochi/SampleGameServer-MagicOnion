using System;
using UniRx;

namespace SampleGame.Domain.Chat
{
    public sealed class ChatSystem
    {
        public IObservable<ChatMessage> OnReceiveMessage => _receiveMessageSubject;
        private readonly Subject<ChatMessage> _receiveMessageSubject = new Subject<ChatMessage>();

        private string _username = "_";

        private readonly IChatServiceGateway _serviceGateway;

        public ChatSystem(IChatServiceGateway serviceGateway)
        {
            _serviceGateway = serviceGateway;

            _serviceGateway.OnReceiveMessage += OnReceiveMessageEventHandler;
            _serviceGateway.OnJoin += OnJoinEventHandler;
            _serviceGateway.OnLeave += OnLeaveEventHandler;
            _serviceGateway.OnUserJoin += OnUserJoinEventHandler;
            _serviceGateway.OnUserLeave += OnUserLeaveEventHandler;
        }

        public void Dispose()
        {
            _serviceGateway.OnReceiveMessage -= OnReceiveMessageEventHandler;
            _serviceGateway.OnJoin -= OnJoinEventHandler;
            _serviceGateway.OnLeave -= OnLeaveEventHandler;
            _serviceGateway.OnUserJoin -= OnUserJoinEventHandler;
            _serviceGateway.OnUserLeave -= OnUserLeaveEventHandler;
        }

        public void SendMessage(string message)
        {
            _serviceGateway.SendMessage(message);
        }

        private void OnReceiveMessageEventHandler(ChatMessage message)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnReceiveMessageEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _receiveMessageSubject.OnNext(message);
        }

        private void OnJoinEventHandler(JoinResult joinResult)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _username = joinResult.Username;
            _receiveMessageSubject.OnNext(new ChatMessage()
            { 
                Message = $"{_username} has been joined.",
            });
        }

        private void OnLeaveEventHandler(JoinResult joinResult)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _username = "_";
            _receiveMessageSubject.OnNext(new ChatMessage()
            { 
                Message = $"{_username} has been left the room.",
            });
        }

        private void OnUserJoinEventHandler(JoinResult joinResult)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnUserJoinEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _receiveMessageSubject.OnNext(new ChatMessage()
            {
                Message = $"{joinResult.Username} has been joined.",
            });
        }

        private void OnUserLeaveEventHandler(JoinResult joinResult)
        {
            // DebugLogger.Log($"[ChatSystemContext] OnUserLeaveEventHandler | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _receiveMessageSubject.OnNext(new ChatMessage()
            {
                Message = $"{joinResult.Username} has been left the room."
            });
        }
    }
}