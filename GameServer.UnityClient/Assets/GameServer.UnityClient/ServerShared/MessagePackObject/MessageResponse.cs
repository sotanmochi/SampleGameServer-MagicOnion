using MessagePack;

namespace GameServer.Shared.MessagePackObject
{
    [MessagePackObject]
    public class MessageResponse
    {
        [Key(0)]
        public string Username { get; set; }

        [Key(1)]
        public string Message { get; set; }
    }
}