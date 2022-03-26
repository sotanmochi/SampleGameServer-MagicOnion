using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Context;
using SampleGame.Gateway;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Application.Lifecycle
{
    public class RoomScreenLifecycle : LifetimeScope
    {
        [Header("Instances")]
        [SerializeField] private RoomUIView _uiView;

        private RoomPresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            var chatService = Container.Resolve<ChatServiceGateway>();
            var multiplayerService = Container.Resolve<MultiplayerServiceGateway>();
            var context = new NetworkServiceContext(chatService, multiplayerService);

            _presenter = new RoomPresenter(_uiView, context);
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