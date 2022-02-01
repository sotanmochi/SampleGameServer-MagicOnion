using MessagePack;

namespace GameServer.Shared.MessagePackObject
{
    [MessagePackObject]
    public class JoinResponse
    {
        [Key(0)]
        public int ClientId { get; set; }

        [Key(1)]
        public string ConnectionId { get; set; }

        [Key(2)]
        public string RoomId { get; set; }

        [Key(3)]
        public string Username { get; set; }
    }
}