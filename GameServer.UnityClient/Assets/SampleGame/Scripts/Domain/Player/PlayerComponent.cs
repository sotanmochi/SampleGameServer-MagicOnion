using UnityEngine;

namespace SampleGame.Domain.Player
{
    public class PlayerComponent : MonoBehaviour
    {
        public bool IsLocalPlayer;
        public ushort PlayerId;
        public Transform Transform => transform;
    }
}