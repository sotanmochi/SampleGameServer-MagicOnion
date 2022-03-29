using System;
using SampleGame.Domain.Camera;
using SampleGame.Domain.Player;

namespace SampleGame.Context
{
    public sealed class CameraContext : IDisposable
    {
        private readonly CameraSystem _cameraSystem;
        private readonly PlayerSpawnSystem _playerSystem;

        public CameraContext(CameraSystem cameraSystem, PlayerSpawnSystem playerSystem)
        {
            _cameraSystem = cameraSystem;
            _playerSystem = playerSystem;

            _playerSystem.OnSpawn += OnPlayerSpawnEventHandler;
            _playerSystem.OnDespawn += OnPlayerDespawnEventHandler;
        }

        public void Dispose()
        {
            _playerSystem.OnSpawn -= OnPlayerSpawnEventHandler;
            _playerSystem.OnDespawn -= OnPlayerDespawnEventHandler;
        }

        private void OnPlayerSpawnEventHandler(PlayerComponent player)
        {
            if (player.IsLocalPlayer)
            {
                _cameraSystem.SetCameraTarget(player.Transform);
            }
        }

        private void OnPlayerDespawnEventHandler(ushort playerId)
        {
            if (_playerSystem.LocalPlayerId == playerId)
            {
                _cameraSystem.SetCameraTarget(null);
            }
        }
    }
}