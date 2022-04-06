using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MagicOnion.Server.Hubs;
using GameServer.Shared.MessagePackObject;
using GameServer.Shared.Streaming;

namespace GameServer.Streaming
{
    /// <summary>
    /// Game server processing. One class instance for one connection.
    /// </summary>
    public sealed partial class GameStreamingHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        private readonly IGameLoopService _gameLoopService;
        private readonly ILogger _logger;

        private IGroup _room;
        private int _clientId = -1;
        private string _username = "";

        public GameStreamingHub(IGameLoopService gameLoopHostedService, ILogger<GameStreamingHub> logger)
        {
            _gameLoopService = gameLoopHostedService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendPlayerPoseAsync(PlayerPoseObject value)
        {
            if (_clientId < 0) { return; }

            // Console.WriteLine($"[GameStreamingHub.SendPlayerPoseAsync] Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            BroadcastExceptSelf(_room).OnReceivePlayerPose(value);
        }

        public async Task JoinAsync(JoinRequest request)
        {
            var roomId = $"GameStreaming_{request.RoomId}";

            if (_clientId >= 0 && roomId == _room?.GroupName) { return; }

            // Console.WriteLine($"[{nameof(GameStreamingHub)}] JoinAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            _room = await Group.AddAsync(roomId);

            var response = new JoinResponse()
            {
                ClientId = -1,
                ConnectionId = Context.ContextId.ToString(),
                RoomId = roomId,
                Username = request.Username,
            };

            if (!_gameLoopService.TryGetGameLoop(roomId, out var gameLoop))
            {
                _logger.LogInformation($"Could not get a game loop for '{roomId}'.");
                BroadcastToSelf(_room).OnLeave(response);
                await _room.RemoveAsync(Context);
                return;
            }

            var newClientId = ClientIdPoolStorage.GetClientId(roomId);
            if (newClientId < 0)
            {
                _logger.LogInformation($"Could not get a client id.");
                BroadcastToSelf(_room).OnLeave(response);
                await _room.RemoveAsync(Context);
                return;
            }

            _clientId = newClientId;
            _username = request.Username;

            Console.WriteLine($"[{nameof(GameStreamingHub)}] JoinAsync | Room: {_room.GroupName}, ClientId: {_clientId}, Username: {_username}, ContextId: {Context.ContextId})");

            response.ClientId = _clientId;
            response.Username = _username;

            Console.WriteLine($"[{nameof(GameStreamingHub)}] Join success: {_clientId}");

            BroadcastToSelf(_room).OnJoin(response);
            BroadcastExceptSelf(_room).OnUserJoin(response);
        }

        public async Task LeaveAsync()
        {
            if (_clientId < 0 || _room is null) { return; }

            // Console.WriteLine($"[{nameof(GameStreamingHub)}] LeaveAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"[{nameof(GameStreamingHub)}] LeaveAsync | Room: {_room.GroupName}, No: {_clientId}, Username: {_username}, Context Id: {Context.ContextId})");

            var response = new JoinResponse()
            {
                ClientId = _clientId,
                ConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            await _room.RemoveAsync(Context);
            ClientIdPoolStorage.ReturnToPool(_room.GroupName, (ushort)_clientId);
            _clientId = -1;

            var memberCount = await _room.GetMemberCountAsync();
            Console.WriteLine($"[{nameof(GameStreamingHub)}] LeaveAsync | Member count: {memberCount}");

            if (memberCount == 0)
            {
                _gameLoopService.ReleaseGameLoop(_room.GroupName);
            }

            BroadcastToSelf(_room).OnLeave(response);
            BroadcastExceptSelf(_room).OnUserLeave(response);
        }

        /// <summary>
        /// Handle connection if needed.
        /// </summary>
        /// <returns></returns>
        protected override ValueTask OnConnecting()
        {
            Console.WriteLine($"[{nameof(GameStreamingHub)}] OnConnecting | Context Id: {Context.ContextId}");
            return CompletedTask;
        }

        /// <summary>
        /// Handle disconnection if needed.
        /// On disconnecting, if automatically removed this connection from group.
        /// </summary>
        /// <returns></returns>
        protected override ValueTask OnDisconnected()
        {
            if (_clientId < 0) { return CompletedTask; }

            Console.WriteLine($"[{nameof(GameStreamingHub)}] OnDisconnected | Room: {_room.GroupName}, No: {_clientId}, Username: {_username}, Context Id: {Context.ContextId})");

            var response = new JoinResponse()
            {
                ClientId = _clientId,
                ConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            ClientIdPoolStorage.ReturnToPool(_room.GroupName, (ushort)_clientId);
            _clientId = -1;

            BroadcastExceptSelf(_room).OnUserLeave(response);

            return CompletedTask;
        }
    }
}