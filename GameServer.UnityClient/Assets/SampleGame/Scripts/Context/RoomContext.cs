using System.Threading;
using Cysharp.Threading.Tasks;
using SampleGame.Utility;

namespace SampleGame.Context
{
    public sealed class RoomContext
    {
        private ChatSystemContext _chatSystemContext;

        public RoomContext(ChatSystemContext chatSystemContext)
        {
            _chatSystemContext = chatSystemContext;
        }

        public async UniTask<bool> Initialize()
        {
            DebugLogger.Log($"[RoomContext] Initialize | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            return await _chatSystemContext.Connect();
        }

        public async UniTask Dispose()
        {
            await _chatSystemContext.Disconnect();
        }

        public async UniTask<bool> Join(string roomId, string username)
        {
            return await _chatSystemContext.Join(roomId, username);
        }

        public async UniTask Leave()
        {
            await _chatSystemContext.Leave();
        }
    }
}