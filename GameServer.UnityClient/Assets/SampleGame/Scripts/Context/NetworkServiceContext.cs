using Cysharp.Threading.Tasks;
using SampleGame.Gateway;

namespace SampleGame.Context
{
    public sealed class NetworkServiceContext
    {
        private readonly ChatServiceGateway _chatService;
        private readonly MultiplayerServiceGateway _multiplayerService;

        public NetworkServiceContext(ChatServiceGateway chatService, MultiplayerServiceGateway multiplayerService)
        {
            _chatService = chatService;
            _multiplayerService = multiplayerService;
        }

        public async UniTask<bool> Connect()
        {
            await _chatService.Connect();
            await _multiplayerService.Connect();
            return true;
        }

        public async UniTask Disconnect()
        {
            await _chatService.Disconnect();
            await _multiplayerService.Disconnect();
        }

        public async UniTask<bool> Join(string roomId, string username)
        {
            await _chatService.Join(roomId, username);
            await _multiplayerService.Join(roomId, username);
            return true;
        }

        public async UniTask Leave()
        {
            await _chatService.Leave();
            await _multiplayerService.Leave();
        }
    }
}