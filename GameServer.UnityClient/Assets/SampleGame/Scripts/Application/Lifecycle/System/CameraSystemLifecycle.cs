using UnityEngine;
using VContainer;
using VContainer.Unity;
using Cinemachine;
using SampleGame.Context;
using SampleGame.Domain.Camera;
using SampleGame.Domain.Player;

namespace SampleGame.Application.Lifecycle
{
    public class CameraSystemLifecycle : LifetimeScope
    {
        [SerializeField] CinemachineVirtualCamera _mainCamera;

        private CameraContext _context;
        private CameraSystem _cameraSystem;

        protected override void Awake()
        {
            base.Awake();

            var playerSpawnSystem = Container.Resolve<PlayerSpawnSystem>();

            _cameraSystem = Container.Resolve<CameraSystem>();
            _cameraSystem.SetMainCamera(_mainCamera);

            _context = new CameraContext(_cameraSystem, playerSpawnSystem);
        }

        protected override void OnDestroy()
        {
            _context.Dispose();
            _context = null;

            _cameraSystem.SetMainCamera(null);
            _cameraSystem = null;

            base.OnDestroy();
        }
    }
}