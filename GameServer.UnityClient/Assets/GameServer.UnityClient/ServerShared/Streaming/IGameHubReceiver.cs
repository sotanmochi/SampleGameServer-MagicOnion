using GameServer.Shared.MessagePackObject;

namespace GameServer.Shared.Streaming
{
    /// <summary>
    /// Server -> Client API
    /// </summary>
    public interface IGameHubReceiver
    {
        void OnReceivePlayerPose(PlayerPoseObject value);
        void OnJoin(JoinResponse response);
        void OnLeave(JoinResponse response);
        void OnUserJoin(JoinResponse response);
        void OnUserLeave(JoinResponse response);
    }
}
