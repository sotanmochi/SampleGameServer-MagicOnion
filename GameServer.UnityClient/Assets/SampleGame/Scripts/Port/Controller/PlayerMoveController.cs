using System;
using UnityEngine;
using UniRx;
using SampleGame.Domain.Player;

namespace SampleGame.Port.Controller
{
    /// <summary>
    /// Input port for player move system
    /// </summary>
    public sealed class PlayerMoveController : IDisposable
    {
        private readonly PlayerMoveSystem _playerMoveSystem;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public PlayerMoveController(PlayerMoveSystem moveSystem)
        {
            _playerMoveSystem = moveSystem;
        }

        public void Initialize()
        {
            Observable.EveryUpdate()
                .Subscribe(_ => 
                {
                    var verticalDiff = Input.GetAxis("Vertical");
                    var horizontalDiff = Input.GetAxis("Horizontal");

                    if (Math.Abs(verticalDiff) > 0.0f || Math.Abs(horizontalDiff) > 0.0f)
                    {
                        _playerMoveSystem.MoveLocalPlayer(verticalDiff, horizontalDiff);
                    }
                })
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}