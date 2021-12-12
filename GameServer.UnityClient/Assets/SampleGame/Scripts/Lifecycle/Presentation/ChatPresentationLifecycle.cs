using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Lifecycle.Presentation
{
    public class ChatPresentationLifecycle : LifecycleBase
    {
        [SerializeField] private ChatMessageUIView _uiView;
        private ChatPresenter _presenter;

        public override async UniTask InitializeAsync()
        {
            var context = ServiceLocator.GetInstance<ChatSystemContext>();
            _presenter = new ChatPresenter(_uiView, context);
            _presenter.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            _presenter.Dispose();
        }
    }
}