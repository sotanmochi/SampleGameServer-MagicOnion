using SampleGame.Domain.Camera;
using SampleGame.Domain.Player;

namespace SampleGame.Context
{
    public sealed class CameraContext
    {
        private readonly CameraSystem _cameraSystem;
        // private readonly PlayerSystem _playerSystem;

        // public CameraContext(CameraSystem cameraSystem, PlayerSystem playerSystem)
        // {
        //     _cameraSystem = cameraSystem;
        //     _playerSystem = playerSystem;

        //     _playerSystem.OnSpawn += OnPlayerSpawnEventHandler;
        //     _playerSystem.OnDespawn += OnPlayerDespawnEventHandler;
        // }

        public void Dispose()
        {
            // _playerSystem.OnSpawn -= OnPlayerSpawnEventHandler;
            // _playerSystem.OnDespawn -= OnPlayerDespawnEventHandler;
        }

        private void OnPlayerSpawnEventHandler(PlayerComponent player)
        {
            _cameraSystem.SetCameraTarget(player.Transform);
        }

        private void OnPlayerDespawnEventHandler(ushort playerId)
        {
            _cameraSystem.SetCameraTarget(null);
        }
    }
}