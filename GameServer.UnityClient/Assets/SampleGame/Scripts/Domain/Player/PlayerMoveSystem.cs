using System;
using System.Collections.Concurrent;
using UnityEngine;
using SampleGame.Utility;

namespace SampleGame.Domain.Player
{
    public sealed class PlayerMoveSystem : IDisposable
    {        
        private readonly ConcurrentQueue<PlayerPose> _poseUpdateEventQueue = new ConcurrentQueue<PlayerPose>();
        private readonly InMemoryStorage<PlayerComponent> _storage;

        private readonly PlayerSpawnSystem _spawnSystem;
        private readonly IMultiplayerServiceGateway _serviceGateway;

        public PlayerMoveSystem
        (
            PlayerSpawnSystem spawnSystem, 
            IMultiplayerServiceGateway serviceGateway
        )
        {
            _spawnSystem = spawnSystem;
            _serviceGateway = serviceGateway;

            _storage = spawnSystem.Storage;

            _serviceGateway.OnPlayerPoseReceive += OnPlayerPoseReceive;
        }

        public void Dispose()
        {
            _serviceGateway.OnPlayerPoseReceive -= OnPlayerPoseReceive;            
        }

        /// <summary>
        /// Runs on main thread
        /// </summary>
        public void Update()
        {
            while (_poseUpdateEventQueue.Count > 0)
            {
                if (_poseUpdateEventQueue.TryDequeue(out var value))
                {
                    var playerId = value.PlayerId;
                    DebugLogger.Log($"[{nameof(PlayerMoveSystem)}] PoseUpdateEvent PlayerId: {playerId}");
                    if (_storage.TryGetValue(playerId, out var player))
                    {
                        if (playerId == _spawnSystem.LocalPlayerId)
                        {
                            _serviceGateway.SendPlayerPose(value);
                        }
                        else
                        {
                            player.Transform.localPosition = value.Position;
                            player.Transform.localRotation = value.Rotation;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Runs on background thread or main thread
        /// </summary>
        /// <param name="value"></param>
        public void UpdateLocalPlayer(Vector3 position, Quaternion rotation)
        {
            Enqueue(new PlayerPose()
            {
                PlayerId = (ushort)_spawnSystem.LocalPlayerId,
                Position = position,
                Rotation = rotation,
            });
        }

        /// <summary>
        /// Runs on background thread or main thread
        /// </summary>
        /// <param name="value"></param>
        public void UpdateLocalPlayer(PlayerPose value)
        {
            value.PlayerId = (ushort)_spawnSystem.LocalPlayerId;
            Enqueue(value);
        }

        /// <summary>
        /// Runs on background thread
        /// </summary>
        private void OnPlayerPoseReceive(PlayerPose value)
        {
            Enqueue(value);
        }

        /// <summary>
        /// Runs on background thread or main thread
        /// </summary>
        /// <param name="value"></param>
        private void Enqueue(PlayerPose value)
        {
            _poseUpdateEventQueue.Enqueue(value);
        }
    }
}