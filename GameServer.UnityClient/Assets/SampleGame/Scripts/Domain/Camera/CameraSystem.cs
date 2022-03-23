using UnityEngine;
using Cinemachine;

namespace SampleGame.Domain.Camera
{
    public sealed class CameraSystem
    {
        private CinemachineVirtualCamera _mainCamera;

        public void SetMainCamera(CinemachineVirtualCamera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        public void SetCameraTarget(Transform target)
        {
            _mainCamera.Follow = target;
            _mainCamera.LookAt = target;
        }
    }
}