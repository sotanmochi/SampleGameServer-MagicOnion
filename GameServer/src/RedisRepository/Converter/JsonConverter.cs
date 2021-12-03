using StackExchange.Redis;
using Utf8Json;

namespace RedisRepository.Converter
{
    internal class JsonConverter<T> : IValueConverter<T>
    {
        public RedisValue Serialize(T value) => JsonSerializer.Serialize<T>(value);
        public T Deserialize(RedisValue value) => JsonSerializer.Deserialize<T>((byte[])value);
    }
}