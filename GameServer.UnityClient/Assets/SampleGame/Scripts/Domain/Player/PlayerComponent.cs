using UnityEngine;

namespace SampleGame.Domain.Player
{
    public class PlayerComponent : MonoBehaviour
    {
        public ushort PlayerId;
        public Transform Transform => transform;
    }
}