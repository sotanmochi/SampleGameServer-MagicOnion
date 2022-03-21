using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;
using SampleGame.Gateway;

namespace SampleGame.Lifecycle.System
{
    public class ChatSystemLifecycle : LifecycleBase
    {
        private ChatSystemContext _systemContext;

        public override async UniTask InitializeAsync()
        {
            var chatServiceGateway = ServiceLocator.GetInstance<ChatServiceGateway>();
            _systemContext = ServiceLocator.Register<ChatSystemContext>(new ChatSystemContext(chatServiceGateway));
            _systemContext.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            await _systemContext.Dispose();
        }
    }
}