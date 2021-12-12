using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using SampleGame.Context;
using SampleGame.UIView;
using SampleGame.Utility;

namespace SampleGame.Presenter
{
    public sealed class RoomPresenter
    {
        private RoomUIView _uiView;
        private RoomContext _context;

        private bool _initialized;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public RoomPresenter(RoomUIView uiView, RoomContext context)
        {
            _uiView = uiView;
            _context = context;
        }

        public void Initialize()
        {
            if (_initialized) return;

            _uiView.OnTriggerJoinRoom
                .Subscribe(joinRequest => 
                {
                    // DebugLogger.Log($"[RoomPresenter] OnTriggerJoinRoom | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                    _context.Join(joinRequest.RoomId, joinRequest.Username).Forget();
                })
                .AddTo(_disposable);

            _uiView.OnTriggerLeaveRoom
                .Subscribe(_ => 
                {
                    // DebugLogger.Log($"[RoomPresenter] OnTriggerJoinRoom | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                    _context.Leave().Forget();
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