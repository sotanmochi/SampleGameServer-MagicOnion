using System;

namespace SampleGame.Domain.Player
{
    public interface IMultiplayerServiceGateway
    {
        void SendPlayerPose(PlayerPose value);
        event Action<PlayerPose> OnPlayerPoseReceive;
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