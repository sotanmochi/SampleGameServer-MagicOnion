using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using GameServer.Shared.Services;

namespace GameServer.UnityClient
{
    public class MyFirstService
    {
        IMyFirstService _service;

        public MyFirstService(ChannelBase channel)
        {
            _service = MagicOnionClient.Create<IMyFirstService>(channel);
        }

        public async Task<int> SumAsync(int x, int y)
        {
            return await _service.SumAsync(x, y);
        }
    }
}