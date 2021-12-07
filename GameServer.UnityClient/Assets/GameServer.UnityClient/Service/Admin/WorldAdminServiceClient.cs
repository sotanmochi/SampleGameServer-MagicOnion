using System;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using GameServer.Shared;
using GameServer.Shared.Services.Admin;

namespace GameServer.UnityClient.Admin
{
    public class WorldAdminServiceClient
    {
        IWorldAdminService _service;

        public WorldAdminServiceClient(Uri uri)
        {
            _service = MagicOnionClient.Create<IWorldAdminService>(GrpcChannelx.ForAddress(uri));
        }

        public async Task<bool> CreateOrUpdateWorld(World world)
        {
            return await _service.CreateOrUpdateWorld(world);
        }
    }
}