using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading;
using Microsoft.Extensions.Logging;

namespace GameServer
{
    public class GameLoop
    {
        public static ConcurrentBag<GameLoop> All { get; } = new ConcurrentBag<GameLoop>();
        private static int _gameLoopSeq = 0;

        public int Id { get; }

        private readonly ILogger _logger;

        /// <summary>
        /// Create a new game loop and register into the LooperPool.
        /// </summary>
        /// <param name="looperPool"></param>
        /// <param name="logger"></param>
        public static void CreateNew(ILogicLooperPool looperPool, ILogger logger)
        {
            var gameLoop = new GameLoop(logger);
            looperPool.RegisterActionAsync(gameLoop.UpdateFrame);
        }

        private GameLoop(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            All.Add(this);
            Id = Interlocked.Increment(ref _gameLoopSeq);

            _logger.LogInformation($"{nameof(GameLoop)}[{Id}]: Register");
        }

        public bool UpdateFrame(in LogicLooperActionContext ctx)
        {
            if (ctx.CancellationToken.IsCancellationRequested)
            {
                // If LooperPool begins shutting down, IsCancellationRequested will be `true`.
                _logger.LogInformation($"{nameof(GameLoop)}[{Id}]: Shutdown");
                return false;
            }

            // ToDo

            return true;
        }
    }
}