using System;
using StackExchange.Redis;

namespace RedisRepository
{
    public class RedisRepositoryOptions
    {
        public ConnectionMultiplexer ConnectionMultiplexer { get; set; }
        public int Db { get; set; } = -1;
    }
}