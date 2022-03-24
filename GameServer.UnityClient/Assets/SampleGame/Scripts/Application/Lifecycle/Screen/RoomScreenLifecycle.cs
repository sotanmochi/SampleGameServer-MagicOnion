using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Application.Lifecycle
{
    public class RoomScreenLifecycle : LifetimeScope
    {
        [Header("Instances")]
        [SerializeField] private RoomUIView _uiView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<RoomPresenter>(Lifetime.Singleton);
            builder.RegisterInstance<RoomUIView>(_uiView);
        }

        protected override void Awake()
        {
            base.Awake();
            var presenter = Container.Resolve<RoomPresenter>();
            presenter.Initialize();
        }
    }
}