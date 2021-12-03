using System;
using StackExchange.Redis;
using RedisRepository.Converter;

namespace RedisRepository
{
    public class RedisValueRepository<T>
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly IValueConverter<T> _converter;

        public RedisValueRepository(RedisRepositoryOptions redisRepositoryOptions)
        {
            _connection = redisRepositoryOptions.ConnectionMultiplexer;
            _database = _connection.GetDatabase(redisRepositoryOptions.Db);

            if (typeof(T) == typeof(string))
            {
                _converter = (IValueConverter<T>)new StringConverter();
            }
            else
            {
                _converter = new JsonConverter<T>();
            }
        }

        public T Find(string key)
        {
            try
            {
                var redisValue = _database.StringGet(key);
                return _converter.Deserialize(redisValue);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public bool Save(string key, T value)
        {
            try
            {
                if (value is null) { return false; }

                var redisValue = _converter.Serialize(value);
                return _database.StringSet(key, redisValue);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Create(string key, T value)
        {
            try
            {
                if (Find(key) is null) { return false; }

                var redisValue = _converter.Serialize(value);
                return _database.StringSet(key, redisValue);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Delete(string key)
        {
            try
            {
                return _database.KeyDelete(key);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}