using System;
using System.Threading;
using UniRx;
using SampleGame.Domain.Chat;
using SampleGame.UIView;
using SampleGame.Utility;

namespace SampleGame.Presenter
{
    public sealed class ChatPresenter : IDisposable
    {
        private ChatMessageUIView _uiView;
        private ChatSystem _system;

        private bool _initialized;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ChatPresenter(ChatMessageUIView uiView, ChatSystem system)
        {
            _uiView = uiView;
            _system = system;
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;

            _uiView.OnTriggerSendingMessage
                .Subscribe(message => 
                {
                    // DebugLogger.Log($"[ChatPresenter] OnTriggerSendingMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                    _system.SendMessage(message);
                })
                .AddTo(_disposable);

            _system.OnReceiveMessage
                .ObserveOnMainThread()
                .Subscribe(data => 
                {
                    DebugLogger.Log($"[ChatPresenter] OnReceiveMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                    if (string.IsNullOrEmpty(data.Username))
                    {
                        _uiView.AppendMessage($"{data.Message}");
                    }
                    else
                    {
                        _uiView.AppendMessage($"{data.Username}: {data.Message}");
                    }
                })
                .AddTo(_disposable);

            _initialized = true;
        }

        public void Dispose()
        {
            _initialized = false;
            _disposable.Dispose();
        }
    }
}