using UnityEngine;
using VContainer;
using VContainer.Unity;
using Cinemachine;
using SampleGame.Context;
using SampleGame.Domain.Camera;

namespace SampleGame.Application.Lifecycle
{
    public class CameraSystemLifecycle : LifetimeScope
    {
        [SerializeField] CinemachineVirtualCamera _mainCamera;

        private CameraSystem _system;
        private CameraContext _context;

        protected override void Awake()
        {
            base.Awake();

            _system = Container.Resolve<CameraSystem>();
            _system.SetMainCamera(_mainCamera);
            
            _context = Container.Resolve<CameraContext>();
        }

        protected override void OnDestroy()
        {
            _system.SetMainCamera(null);
            _system = null;

            base.OnDestroy();
        }
    }
}