using MessagePack;

namespace GameServer.Shared.MessagePackObject
{
    [MessagePackObject]
    public struct JoinResponse
    {
        [Key(0)]
        public uint ClientNumber { get; set; }

        [Key(1)]
        public string ClientConnectionId { get; set; }

        [Key(2)]
        public string RoomId { get; set; }

        [Key(3)]
        public string Username { get; set; }
    }
}