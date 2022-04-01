using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameServer
{
    class GameLoopHostedService : IHostedService
    {
        private readonly ILogicLooperPool _looperPool;
        private readonly ILogger _logger;

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

            // Create a new game loop and register into the LooperPool.
            GameLoop.CreateNew(_looperPool, _logger);

            _logger.LogInformation($"GameLoopHostedService is started. (Loopers={_looperPool.Loopers.Count}; TargetFrameRate={_looperPool.Loopers[0].TargetFrameRate:0}fps)");

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
    }
}