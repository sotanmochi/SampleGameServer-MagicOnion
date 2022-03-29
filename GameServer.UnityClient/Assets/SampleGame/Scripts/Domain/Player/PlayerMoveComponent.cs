using UnityEngine;

namespace SampleGame.Domain.Player
{
    public class PlayerMoveComponent : PlayerComponent
    {
        public float MoveSpeed = 3.5f;
        public float RotationSpeed = 180f;

        public void Move(float forward, float right)
        {
            var forwardDirection = transform.forward;
            var rightDirection = transform.right;

            Vector3 moveDirection = Vector3.Normalize(forward * forwardDirection + right * rightDirection);

            if(moveDirection.sqrMagnitude > 0.01f)
            {
                forwardDirection = Vector3.Slerp(
                    forwardDirection,
                    moveDirection,
                    RotationSpeed * Time.deltaTime / Vector3.Angle(forwardDirection, moveDirection)
                );
                transform.LookAt(transform.position + forwardDirection);
            }

            transform.Translate(moveDirection * MoveSpeed * Time.deltaTime, Space.World);
        }
    }
}