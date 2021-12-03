using System;
using System.Collections.Generic;
using MagicOnion;
using MagicOnion.Server;
using RedisRepository;
using GameServer.Shared;
using GameServer.Shared.Services;

namespace GameServer.Services
{
    public class WorldService : ServiceBase<IWorldService>, IWorldService
    {
        private readonly RedisSetRepository<string> _redisSetRepository;
        private readonly RedisValueRepository<World> _redisValueRepository;

        public WorldService(RedisSetRepository<string> redisSetRepository, RedisValueRepository<World> redisValueRepository)
        {
            _redisSetRepository = redisSetRepository;
            _redisValueRepository = redisValueRepository;
        }

        public async UnaryResult<List<string>> FindWorldIdOrderByRank()
        {
            return _redisSetRepository.FindAllOrderByRank("World").ToList();
        }

        public async UnaryResult<World> FindWorld(string worldId)
        {
            return _redisValueRepository.Find(worldId);
        }
    }
}