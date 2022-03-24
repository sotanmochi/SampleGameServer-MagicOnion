using UnityEngine;
using VContainer;
using VContainer.Unity;
using SampleGame.Domain.Player;

namespace SampleGame.Application.Lifecycle
{
    public sealed class PlayerSystemLifecycle : LifetimeScope
    {
        [Header("Instances")]
        [SerializeField] PlayerComponent _playerPrefab;
        [SerializeField] Transform _spawnRoot;

        private PlayerSpawnSystem _spawnSystem;
        private PlayerMoveSystem _moveSystem;

        protected override void Awake()
        {
            base.Awake();

            _spawnSystem = Container.Resolve<PlayerSpawnSystem>();
            _spawnSystem.SetPlayerPrefab(_playerPrefab);
            _spawnSystem.SetParentTransform(_spawnRoot);

            _moveSystem = Container.Resolve<PlayerMoveSystem>();
        }

        protected override void OnDestroy()
        {
            _spawnSystem.SetPlayerPrefab(null);
            _spawnSystem.SetParentTransform(null);

            base.OnDestroy();
        }

        private void Update()
        {
            _moveSystem.Update();
        }
    }
}