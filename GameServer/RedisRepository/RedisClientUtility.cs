using System;
using StackExchange.Redis;
using RedisRepository.Converter;

namespace RedisRepository
{
    public class RedisClientUtility
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly int _db;

        public RedisClientUtility(RedisRepositoryOptions redisRepositoryOptions)
        {
            _connection = redisRepositoryOptions.ConnectionMultiplexer;
            _database = _connection.GetDatabase(redisRepositoryOptions.Db);
            _db = redisRepositoryOptions.Db;
        }

        public IEnumerable<T> FindAll<T>(string keyPattern = "*")
        {
            var valueType = typeof(T);

            IValueConverter<T> converter = new JsonConverter<T>();
            if (valueType == typeof(string))
            {
                converter = (IValueConverter<T>)new StringConverter();
            }

            var endpoint = _connection.GetEndPoints(true).FirstOrDefault();
            var keys = _connection.GetServer(endpoint).Keys(_db, keyPattern);

            var redisValues = _database.StringGet(keys.ToArray());

            var values = redisValues.Where(redisValue => redisValue.HasValue)
                                    .Select<RedisValue, T>(redisValue => 
                                    {
                                        try
                                        {
                                            return converter.Deserialize(redisValue.ToString());
                                        }
                                        catch (Exception e)
                                        {
                                            return default(T);
                                        }
                                    })
                                    .Where(value => value != null);

            return values;
        }
    }
}