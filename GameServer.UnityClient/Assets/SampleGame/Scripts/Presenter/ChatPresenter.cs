using System.Threading;
using UniRx;
using SampleGame.Context;
using SampleGame.UIView;
using SampleGame.Utility;

namespace SampleGame.Presenter
{
    public sealed class ChatPresenter
    {
        private ChatMessageUIView _uiView;
        private ChatSystemContext _context;

        private bool _initialized;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ChatPresenter(ChatMessageUIView uiView, ChatSystemContext context)
        {
            _uiView = uiView;
            _context = context;
        }

        public void Initialize()
        {
            if (_initialized) return;

            _uiView.OnTriggerSendingMessage
                .Subscribe(message => 
                {
                    // DebugLogger.Log($"[ChatPresenter] OnTriggerSendingMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                    _context.SendMessage(message);
                })
                .AddTo(_disposable);

            _context.OnReceiveMessage
                .Subscribe(data => 
                {
                    // DebugLogger.Log($"[ChatPresenter] OnReceiveMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
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