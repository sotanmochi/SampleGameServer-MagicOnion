using MessagePack;
using UnityEngine;

namespace GameServer.Shared.MessagePackObject
{
    [MessagePackObject]
    public class PlayerPose
    {
        [Key(0)]
        public ushort PlayerId;

        [Key(1)]
        public Vector3 Position;

        [Key(2)]
        public Quaternion Rotation;
    }
}