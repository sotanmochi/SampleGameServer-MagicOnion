using StackExchange.Redis;

namespace RedisRepository.Converter
{
    internal interface IValueConverter<T>
    {
        RedisValue Serialize(T value);
        T Deserialize(RedisValue value);
    }
}