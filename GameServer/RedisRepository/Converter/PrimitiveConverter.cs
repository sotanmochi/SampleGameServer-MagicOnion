using StackExchange.Redis;

namespace RedisRepository.Converter
{
    internal class StringConverter : IValueConverter<string>
    {
        public RedisValue Serialize(string value) => value;
        public string Deserialize(RedisValue value) => value;
    }
}