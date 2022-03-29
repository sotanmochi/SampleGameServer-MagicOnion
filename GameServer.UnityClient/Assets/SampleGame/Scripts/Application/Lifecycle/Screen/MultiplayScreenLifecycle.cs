using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Domain.Player;
using SampleGame.Port.Controller;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Application.Lifecycle
{
    public class MultiplayScreenLifecycle : LifetimeScope
    {
        private PlayerMoveController _controller;

        protected override void Awake()
        {
            base.Awake();

            var system = Container.Resolve<PlayerMoveSystem>();

            _controller = new PlayerMoveController(system);
            _controller.Initialize();
        }

        protected override void OnDestroy()
        {
            _controller.Dispose();
            _controller = null;

            base.OnDestroy();
        }
    }
}