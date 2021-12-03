using MagicOnion;
using MagicOnion.Server;
using RedisRepository;
using GameServer.Shared;
using GameServer.Shared.Services.Admin;

namespace GameServer.Services.Admin
{
    public class WorldAdminService : ServiceBase<IWorldAdminService>, IWorldAdminService
    {
        private readonly RedisValueRepository<World> _redisValueRepository;

        public WorldAdminService(RedisValueRepository<World> redisValueRepository)
        {
            _redisValueRepository = redisValueRepository;
        }

        public async UnaryResult<bool> CreateOrUpdateWorld(World world)
        {
            return _redisValueRepository.Save(world.WorldId, world);
        }
    }
}