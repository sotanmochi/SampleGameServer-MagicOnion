using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Domain.Chat;
using SampleGame.Gateway;

namespace SampleGame.Lifecycle.System
{
    public class ChatSystemLifecycle : LifecycleBase
    {
        private ChatSystem _system;

        public override async UniTask InitializeAsync()
        {
            var chatServiceGateway = ServiceLocator.GetInstance<ChatServiceGateway>();
            _system = ServiceLocator.Register<ChatSystem>(new ChatSystem(chatServiceGateway));
        }

        public override async UniTask DisposeAsync()
        {
            _system.Dispose();
        }
    }
}