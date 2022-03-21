using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using SampleGame.Domain.Chat;
using SampleGame.Domain.Network;
using SampleGame.Gateway;
using SampleGame.Utility;

namespace SampleGame.Context
{
    public sealed class ChatSystemContext
    {
        public IObservable<ChatMessage> OnReceiveMessage => _receiveMessageSubject;
        private readonly Subject<ChatMessage> _receiveMessageSubject = new Subject<ChatMessage>();

        private string _username;

        private readonly ChatServiceGateway _chatService;

        public ChatSystemContext(ChatServiceGateway chatService)
        {
            _chatService = chatService;
        }

        public void Initialize()
        {
            _chatService.OnJoin += OnJoinEventHandler;
            _chatService.OnLeave += OnLeaveEventHandler;
            _chatService.OnUserJoin += OnUserJoinEventHandler;
            _chatService.OnUserLeave += OnUserLeaveEventHandler;
            _chatService.OnReceiveMessage += OnReceiveMessageEventHandler;           
        }

        public async UniTask Dispose()
        {
            _receiveMessageSubject.Dispose();
            _chatService.OnJoin -= OnJoinEventHandler;
            _chatService.OnLeave -= OnLeaveEventHandler;
            _chatService.OnUserJoin -= OnUserJoinEventHandler;
            _chatService.OnUserLeave -= OnUserLeaveEventHandler;
            _chatService.OnReceiveMessage -= OnReceiveMessageEventHandler;
        }

        public void SendMessage(string message)
        {
            DebugLogger.Log($"[ChatSystemContext] SendMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _chatService.SendMessage(message);
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