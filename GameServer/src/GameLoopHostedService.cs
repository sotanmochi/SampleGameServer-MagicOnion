using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace GameServer
{
    public interface IGameLoopService
    {
        bool TryGetGameLoop(string key, out GameLoop gameLoop);
        void ReleaseGameLoop(string key);
    }

    public class GameLoopHostedService : IHostedService, IGameLoopService
    {
        private readonly ILogicLooperPool _looperPool;
        private readonly ILogger _logger;

        private ConcurrentQueue<GameLoop> _gameLoopPool = new ConcurrentQueue<GameLoop>();
        private ConcurrentDictionary<string, GameLoop> _activeGameLoops = new ConcurrentDictionary<string, GameLoop>();

        public GameLoopHostedService(ILogicLooperPool looperPool, ILogger<GameLoopHostedService> logger)
        {
            _looperPool = looperPool ?? throw new ArgumentNullException(nameof(looperPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Register update action immediately.
            _ = _looperPool.RegisterActionAsync((in LogicLooperActionContext ctx) =>
            {
                if (ctx.CancellationToken.IsCancellationRequested)
                {
                    // If LooperPool begins shutting down, IsCancellationRequested will be `true`.
                    _logger.LogInformation("GameLoopHostedService will be shutdown soon. The registered action is shutting down gracefully.");
                    return false;
                }

                return true;
            });

            // Create game loops
            var gameLoopCount = _looperPool.Loopers.Count;
            for (int i = 0; i < gameLoopCount; i++)
            {
                var gameLoop = new GameLoop(_logger);
                _looperPool.RegisterActionAsync(gameLoop.UpdateFrame);
                _gameLoopPool.Enqueue(gameLoop);
            }

            var message = $"GameLoopHostedService is started. "
                        + $"("
                        + $"TargetFrameRate={_looperPool.Loopers[0].TargetFrameRate:0}fps; "
                        + $"Loopers={_looperPool.Loopers.Count}; "
                        + $"GameLoops={_gameLoopPool.Count}"
                        + $")";

            _logger.LogInformation(message);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GameLoopHostedService is shutting down. Waiting for loops.");

            // Shutdown gracefully the LooperPool after 5 seconds.
            await _looperPool.ShutdownAsync(TimeSpan.FromSeconds(5));

            // Count remained actions in the LooperPool.
            var remainedActions = _looperPool.Loopers.Sum(x => x.ApproximatelyRunningActions);
            _logger.LogInformation($"{remainedActions} actions are remained in loop.");
        }

        /// <summary>
        /// Get a game loop.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gameLoop"></param>
        /// <returns></returns>
        public bool TryGetGameLoop(string key, out GameLoop gameLoop)
        {
            if (_activeGameLoops.TryGetValue(key, out gameLoop))
            {
                return true;
            }

            if (_gameLoopPool.TryDequeue(out gameLoop))
            {
                gameLoop.IsActive = true;
                return _activeGameLoops.TryAdd(key, gameLoop);
            }

            return false;
        }

        /// <summary>
        /// Retern a game loop to the pool.
        /// </summary>
        /// <param name="key"></param>
        public void ReleaseGameLoop(string key)
        {
            if (_activeGameLoops.TryRemove(key, out var gameLoop))
            {
                gameLoop.IsActive = false;
                _gameLoopPool.Enqueue(gameLoop);
            }
        }
    }
}