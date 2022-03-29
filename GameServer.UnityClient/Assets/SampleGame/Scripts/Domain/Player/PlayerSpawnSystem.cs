using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Utility;

namespace SampleGame.Domain.Player
{
    public sealed class PlayerSpawnSystem : IDisposable
    {
        public event Action<PlayerComponent> OnSpawn;
        public event Action<ushort> OnDespawn;

        public int LocalPlayerId => _localPlayerId;
        private int _localPlayerId;

        public PlayerComponent LocalPlayer => _localPlayer;
        private PlayerComponent _localPlayer;

        private PlayerComponent _prefab;
        private Transform _parent;

        public readonly InMemoryStorage<PlayerComponent> Storage;

        private readonly IMultiplayerServiceGateway _serviceGateway;

        public PlayerSpawnSystem(IMultiplayerServiceGateway serviceGateway)
        {
            Storage = new InMemoryStorage<PlayerComponent>(1024);

            _serviceGateway = serviceGateway;

            _serviceGateway.OnJoin += OnJoinEventHandler;
            _serviceGateway.OnLeave += OnLeaveEventHandler;
            _serviceGateway.OnUserJoin += OnUserJoinEventHandler;
            _serviceGateway.OnUserLeave += OnUserLeaveEventHandler;
        }

        public void Dispose()
        {
            _serviceGateway.OnJoin -= OnJoinEventHandler;
            _serviceGateway.OnLeave -= OnLeaveEventHandler;
            _serviceGateway.OnUserJoin -= OnUserJoinEventHandler;
            _serviceGateway.OnUserLeave -= OnUserLeaveEventHandler;
        }

        public void SetPlayerPrefab(PlayerComponent prefab)
        {
            _prefab = prefab;
        }

        public void SetParentTransform(Transform parent)
        {
            _parent = parent;
        }

        public async void Spawn(ushort id, bool isLocalPlayer)
        {
            await UniTask.SwitchToMainThread();
            DebugLogger.Log($"[{nameof(PlayerSpawnSystem)}] Spawn | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            if (Storage.TryGetValue(id, out var player))
            {
                if (player is null)
                {
                    player = GameObject.Instantiate<PlayerComponent>(_prefab, Vector3.zero, Quaternion.identity, _parent);
                    player.PlayerId = id;
                    player.IsLocalPlayer = isLocalPlayer;
                    Storage.TryAdd(id, player);
                }
                else
                {
                    DebugLogger.LogWarning($"[{nameof(PlayerSpawnSystem)}] Player '[{id}]' already exists.");
                }

                if (isLocalPlayer)
                {
                    _localPlayerId = id;
                    _localPlayer = player;
                }

                DebugLogger.Log($"[{nameof(PlayerSpawnSystem)}] Player[{id}]: {player.name}");

                OnSpawn?.Invoke(player);
            }
            else
            {
                DebugLogger.LogWarning($"[{nameof(PlayerSpawnSystem)}] Index is out of range. Player Id: {id}");
            }
        }

        public async void Despawn(ushort id)
        {
            await UniTask.SwitchToMainThread();
            DebugLogger.Log($"[{nameof(PlayerSpawnSystem)}] Despawn | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            if (id == _localPlayerId)
            {
                _localPlayerId = -1;
            }

            OnDespawn?.Invoke(id);

            if (Storage.TryRemove(id, out var player))
            {
                GameObject.Destroy(player);
            }
        }

        private void OnJoinEventHandler(JoinResult joinResult)
        {
            if (joinResult.ClientId < 0){ return; }

            Spawn((ushort)joinResult.ClientId, true);
        }

        private void OnLeaveEventHandler(JoinResult joinResult)
        {
            if (joinResult.ClientId < 0){ return; }

            Despawn((ushort)joinResult.ClientId);
        }

        private void OnUserJoinEventHandler(JoinResult joinResult)
        {
            if (joinResult.ClientId < 0){ return; }

            Spawn((ushort)joinResult.ClientId, false);
        }

        private void OnUserLeaveEventHandler(JoinResult joinResult)
        {
            if (joinResult.ClientId < 0){ return; }

            Despawn((ushort)joinResult.ClientId);
        }
    }
}