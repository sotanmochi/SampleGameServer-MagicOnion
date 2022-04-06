using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading;
using Microsoft.Extensions.Logging;

namespace GameServer
{
    public class GameLoop
    {
        public bool IsActive;
        public int Id => _id;

        private static int _gameLoopSequence = 0;

        private readonly int _id;
        private readonly ILogger _logger;

        public GameLoop(ILogger logger)
        {
            _id = Interlocked.Increment(ref _gameLoopSequence);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation($"{nameof(GameLoop)}[{_id}]: Register");
        }

        public bool UpdateFrame(in LogicLooperActionContext ctx)
        {
            if (ctx.CancellationToken.IsCancellationRequested)
            {
                // If LooperPool begins shutting down, IsCancellationRequested will be `true`.
                _logger.LogInformation($"{nameof(GameLoop)}[{_id}]: Shutdown");
                return false;
            }

            if (!IsActive)
            {
                return true;
            }

            // ToDo
            _logger.LogInformation($"{nameof(GameLoop)}[{_id}]: LooperId: {ctx.Looper.Id}");

            return true;
        }
    }
}