using MessagePack;

namespace GameServer.Shared
{
    [MessagePackObject]
    public class World
    {
        [Key(0)]
        public string WorldId { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Description { get; set; }
    }
}