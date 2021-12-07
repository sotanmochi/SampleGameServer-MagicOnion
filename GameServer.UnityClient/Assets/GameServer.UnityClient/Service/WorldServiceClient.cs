using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using GameServer.Shared;
using GameServer.Shared.Services;

namespace GameServer.UnityClient
{
    public class WorldServiceClient
    {
        IWorldService _service;

        public WorldServiceClient(Uri uri)
        {
            _service = MagicOnionClient.Create<IWorldService>(GrpcChannelx.ForAddress(uri));
        }

        public async Task<List<string>> FindWorldIdOrderByRank()
        {
            return await _service.FindWorldIdOrderByRank();
        }

        public async Task<World> FindWorld(string worldId)
        {
            return await _service.FindWorld(worldId);
        }
    }
}