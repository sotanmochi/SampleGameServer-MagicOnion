using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Domain.Chat;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Application.Lifecycle
{
    public class ChatScreenLifecycle : LifetimeScope
    {
        [Header("Instances")]
        [SerializeField] private ChatMessageUIView _uiView;

        private ChatPresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            var system = Container.Resolve<ChatSystem>();

            _presenter = new ChatPresenter(_uiView, system);
            _presenter.Initialize();
        }

        protected override void OnDestroy()
        {
            _presenter.Dispose();
            _presenter = null;

            base.OnDestroy();
        }
    }
}