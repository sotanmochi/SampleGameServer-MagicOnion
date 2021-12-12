using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;

namespace SampleGame.Lifecycle.System
{
    public class RoomLifecycle : LifecycleBase
    {
        private RoomContext _context;

        public override async UniTask InitializeAsync()
        {
            var chatSystem = ServiceLocator.GetInstance<ChatSystemContext>();
            _context = ServiceLocator.Register<RoomContext>(new RoomContext(chatSystem));
            await _context.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            await _context.Dispose();
        }
    }
}