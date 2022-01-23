using MessagePack;
using UnityEngine;

namespace GameServer.Shared.MessagePackObject
{
    [MessagePackObject]
    public struct PlayerState
    {
        [Key(0)]
        public ushort PlayerId;

        [Key(1)]
        public Vector3 Position;

        [Key(2)]
        public Vector3 RotationAngles;
    }
}