using MessagePack;

namespace GameServer.Shared.MessagePackObject
{
    [MessagePackObject]
    public class JoinRequest
    {
        [Key(0)]
        public string RoomId { get; set; }

        [Key(1)]
        public string Username { get; set; }
    }
}