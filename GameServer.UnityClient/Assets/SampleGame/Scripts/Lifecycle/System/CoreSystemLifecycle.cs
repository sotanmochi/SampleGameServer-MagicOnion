using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;
using SampleGame.Gateway;

namespace SampleGame.Lifecycle.System
{
    public class CoreSystemLifecycle : LifecycleBase
    {
        [SerializeField] string _address = "http://localhost:5000";

        private NetworkServiceContext _networkServiceContext;

        private ChatServiceGateway _chatServiceGateway;
        private MultiplayerServiceGateway _multiplayerServiceGateway;

        public override async UniTask InitializeAsync()
        {
            _chatServiceGateway = ServiceLocator.Register<ChatServiceGateway>(new ChatServiceGateway(_address));
            _multiplayerServiceGateway = ServiceLocator.Register<MultiplayerServiceGateway>(new MultiplayerServiceGateway(_address));

            _chatServiceGateway.Initialize();
            _multiplayerServiceGateway.Initialize();

            _networkServiceContext = ServiceLocator.Register<NetworkServiceContext>(
                                        new NetworkServiceContext(_chatServiceGateway, _multiplayerServiceGateway));
        }

        public override async UniTask DisposeAsync()
        {
            _networkServiceContext = null;
            await _chatServiceGateway.Dispose();
            await _multiplayerServiceGateway.Dispose();
        }
    }
}