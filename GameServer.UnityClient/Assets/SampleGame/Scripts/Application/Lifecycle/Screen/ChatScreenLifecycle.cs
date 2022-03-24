using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Application.Lifecycle
{
    public class ChatScreenLifecycle : LifetimeScope
    {
        [Header("Instances")]
        [SerializeField] private ChatMessageUIView _uiView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ChatPresenter>(Lifetime.Singleton);
            builder.RegisterInstance<ChatMessageUIView>(_uiView);
        }

        protected override void Awake()
        {
            base.Awake();
            var presenter = Container.Resolve<ChatPresenter>();
            presenter.Initialize();
        }
    }
}