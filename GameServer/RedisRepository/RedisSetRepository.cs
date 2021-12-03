using System;
using StackExchange.Redis;
using RedisRepository.Converter;

namespace RedisRepository
{
    public class RedisSetRepository<T>
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly IValueConverter<T> _converter;

        public RedisSetRepository(RedisRepositoryOptions redisRepositoryOptions)
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

        public IEnumerable<T> FindAllOrderByRank(string key, long start = 0, long stop = -1, bool descending = false)
        {
            try
            {
                var order = descending ? Order.Descending : Order.Ascending;

                var redisValues = _database.SortedSetRangeByRank(key, start, stop, order);

                var members = redisValues.Where(redisValue => redisValue.HasValue)
                                        .Select(redisValue => 
                                        {
                                            try
                                            {
                                                return _converter.Deserialize(redisValue);
                                            }
                                            catch (Exception e)
                                            {
                                                return default(T);
                                            }
                                        })
                                        .Where(value => value != null);

                return members;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Add(string key, T member, double score)
        {
            try
            {
                var value = _converter.Serialize(member);
                return _database.SortedSetAdd(key, value, score);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Remove(string key, T member)
        {
            try
            {
                var value = _converter.Serialize(member);
                return _database.SortedSetRemove(key, value);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}