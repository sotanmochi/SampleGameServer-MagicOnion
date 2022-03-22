using System;

namespace SampleGame.Domain.Chat
{
    public interface IChatServiceGateway
    {
        void SendMessage(string message);
        event Action<ChatMessage> OnReceiveMessage;
        event Action<JoinResult> OnJoin;
        event Action<JoinResult> OnLeave;
        event Action<JoinResult> OnUserJoin;
        event Action<JoinResult> OnUserLeave;
    }

    public class JoinResult
    {
        public int ClientId { get; set; }
        public string RoomId { get; set; }
        public string Username { get; set; }
    }
}