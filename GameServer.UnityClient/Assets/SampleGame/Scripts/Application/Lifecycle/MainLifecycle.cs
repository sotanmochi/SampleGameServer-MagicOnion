using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Configuration;
using SampleGame.Context;
using SampleGame.Domain.Camera;
using SampleGame.Domain.Chat;
using SampleGame.Domain.Player;
using SampleGame.Gateway;

namespace SampleGame.Application.Lifecycle
{
    public sealed class MainLifecycle : LifetimeScope
    {
        [Header("Configurations")]
        [SerializeField] ChatServiceConfigurationBase _chatServiceConfiguration;
        [SerializeField] MultiplayerServiceConfigurationBase _multiplayerServiceConfiguration;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigurations(builder);
            RegisterServices(builder);
        }

        protected override void Awake()
        {
            base.Awake();
            var config = Container.Resolve<ChatServiceConfiguration>();
            Debug.Log($"Config.Address: {config.Address}");
        }

        private void RegisterConfigurations(IContainerBuilder builder)
        {
            builder.RegisterInstance(_chatServiceConfiguration).As(_chatServiceConfiguration.GetType());
            builder.RegisterInstance(_multiplayerServiceConfiguration).As(_multiplayerServiceConfiguration.GetType());
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<CameraContext>(Lifetime.Singleton);
            builder.Register<NetworkServiceContext>(Lifetime.Singleton);

            builder.Register<CameraSystem>(Lifetime.Singleton);
            builder.Register<ChatSystem>(Lifetime.Singleton);
            builder.Register<PlayerSpawnSystem>(Lifetime.Singleton);
            builder.Register<PlayerMoveSystem>(Lifetime.Singleton);

            builder.Register<ChatServiceGateway>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<MultiplayerServiceGateway>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}