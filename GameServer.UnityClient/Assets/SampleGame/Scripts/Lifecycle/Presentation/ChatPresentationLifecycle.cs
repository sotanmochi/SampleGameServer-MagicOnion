using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Domain.Chat;
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
            var system = ServiceLocator.GetInstance<ChatSystem>();
            _presenter = new ChatPresenter(_uiView, system);
            _presenter.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            _presenter.Dispose();
        }
    }
}