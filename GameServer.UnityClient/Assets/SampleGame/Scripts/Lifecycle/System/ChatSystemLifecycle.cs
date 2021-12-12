using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;

namespace SampleGame.Lifecycle.System
{
    public class ChatSystemLifecycle : LifecycleBase
    {
        [SerializeField] string _address = "http://localhost:5000";
        private ChatSystemContext _systemContext;

        public override async UniTask InitializeAsync()
        {
            _systemContext = ServiceLocator.Register<ChatSystemContext>(new ChatSystemContext(_address));
            _systemContext.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            await _systemContext.Dispose();
        }
    }
}