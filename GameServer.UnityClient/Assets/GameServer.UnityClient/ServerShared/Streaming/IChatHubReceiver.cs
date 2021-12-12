using GameServer.Shared.MessagePackObject;

namespace GameServer.Shared.Streaming
{
    /// <summary>
    /// Server -> Client API
    /// </summary>
    public interface IChatHubReceiver
    {
        void OnJoin(JoinResponse response);
        void OnLeave(JoinResponse response);
        void OnUserJoin(JoinResponse response);
        void OnUserLeave(JoinResponse response);
        void OnReceiveMessage(MessageResponse message);
    }
}
