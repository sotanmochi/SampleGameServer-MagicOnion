using System;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using GameServer.Shared.Services;

namespace GameServer.UnityClient
{
    public class MyFirstService
    {
        IMyFirstService _service;

        public MyFirstService(Uri uri)
        {
            _service = MagicOnionClient.Create<IMyFirstService>(GrpcChannelx.ForAddress(uri));
        }

        public async Task<int> SumAsync(int x, int y)
        {
            return await _service.SumAsync(x, y);
        }
    }
}