using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;
using SampleGame.Gateway;

namespace SampleGame.Lifecycle.System
{
    public class ChatSystemLifecycle : LifecycleBase
    {
        [SerializeField] string _address = "http://localhost:5000";

        private ChatServiceGateway _chatServiceGateway;
        private ChatSystemContext _systemContext;

        public override async UniTask InitializeAsync()
        {
            _chatServiceGateway = new ChatServiceGateway(_address);
            _chatServiceGateway.Initialize();

            _systemContext = ServiceLocator.Register<ChatSystemContext>(new ChatSystemContext(_chatServiceGateway));
            _systemContext.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            await _systemContext.Dispose();
            await _chatServiceGateway.Dispose();
        }
    }
}