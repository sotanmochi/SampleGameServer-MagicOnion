using System;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using GameServer.Shared.MessagePackObject;
using GameServer.Shared.Streaming;

namespace GameServer.Streaming
{
    /// <summary>
    /// Game server processing.
    /// One class instance for one connection.
    /// </summary>
    public sealed class GameStreamingHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        private IGroup _room;
        private string _username;
        private int _clientId = -1;

        public async Task SendPlayerPoseAsync(PlayerPoseObject value)
        {
            if (_clientId < 0) { return; }

            // Console.WriteLine($"[GameStreamingHub.SendPlayerPoseAsync] Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            BroadcastExceptSelf(_room).OnReceivePlayerPose(value);
        }

        public async Task JoinAsync(JoinRequest request)
        {
            if (_clientId >= 0 && request.RoomId == _room?.GroupName) { return; }

            // Console.WriteLine($"[{nameof(GameStreamingHub)}] JoinAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            var roomId = $"GameStreaming_{request.RoomId}";
            ClientIdPoolStorage.CreateOrGetPool(roomId, 10);

            var newClientId = ClientIdPoolStorage.GetClientId(roomId);
            // Console.WriteLine($"[{nameof(GameStreamingHub)}] JoinAsync | ClientId: {newClientId}");
            if (newClientId < 0)
            {
                var failedResponse = new JoinResponse()
                {
                    ClientId = _clientId,
                    ConnectionId = Context.ContextId.ToString(),
                    RoomId = roomId,
                    Username = request.Username,
                };

                _room = await Group.AddAsync(roomId);
                BroadcastToSelf(_room).OnLeave(failedResponse);
                await _room.RemoveAsync(Context);

                return;
            }

            _room = await Group.AddAsync(roomId);
            _username = request.Username;
            _clientId = newClientId;

            Console.WriteLine($"[{nameof(GameStreamingHub)}] JoinAsync | Room: {_room.GroupName}, ClientId: {_clientId}, Username: {_username}, ContextId: {Context.ContextId})");

            var successResponse = new JoinResponse()
            {
                ClientId = _clientId,
                ConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            Console.WriteLine($"[{nameof(GameStreamingHub)}] Join success: {_clientId}");

            BroadcastToSelf(_room).OnJoin(successResponse);
            BroadcastExceptSelf(_room).OnUserJoin(successResponse);
        }

        public async Task LeaveAsync()
        {
            if (_clientId < 0) { return; }

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