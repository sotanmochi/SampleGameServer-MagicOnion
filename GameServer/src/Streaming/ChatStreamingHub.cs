using System;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using GameServer.Shared.MessagePackObject;
using GameServer.Shared.Streaming;

namespace GameServer.Streaming
{
    /// <summary>
    /// Chat server processing.
    /// One class instance for one connection.
    /// </summary>
    public sealed class ChatStreamingHub : StreamingHubBase<IChatHub, IChatHubReceiver>, IChatHub
    {
        private IGroup _room;
        private string _username;
        private int _clientId = -1;

        public async Task JoinAsync(JoinRequest request)
        {
            if (_clientId >= 0 && request.RoomId == _room?.GroupName) { return; }

            // Console.WriteLine($"[ChatStreamingHub] JoinAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            ClientIdPoolStorage.CreateOrGetPool(request.RoomId, 10);

            var newClientId = ClientIdPoolStorage.GetClientId(request.RoomId);
            // Console.WriteLine($"[ChatStreamingHub] JoinAsync | ClientId: {newClientId}");
            if (newClientId < 0)
            {
                var failedResponse = new JoinResponse()
                {
                    ClientId = _clientId,
                    ConnectionId = Context.ContextId.ToString(),
                    RoomId = request.RoomId,
                    Username = request.Username,
                };

                _room = await Group.AddAsync(request.RoomId);
                BroadcastToSelf(_room).OnLeave(failedResponse);
                await _room.RemoveAsync(Context);

                return;
            }

            _room = await Group.AddAsync(request.RoomId);
            _username = request.Username;
            _clientId = newClientId;

            // Console.WriteLine($"[ChatStreamingHub] JoinAsync | Room: {_room.GroupName}, ClientId: {_clientId}, Username: {_username}, ContextId: {Context.ContextId})");

            var successResponse = new JoinResponse()
            {
                ClientId = _clientId,
                ConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            // Console.WriteLine($"[ChatStreamingHub] Join success: {_clientId}");

            BroadcastToSelf(_room).OnJoin(successResponse);
            BroadcastExceptSelf(_room).OnUserJoin(successResponse);
        }

        public async Task LeaveAsync()
        {
            if (_clientId < 0) { return; }

            // Console.WriteLine($"[ChatStreamingHub] LeaveAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            // Console.WriteLine($"[ChatStreamingHub] LeaveAsync | Room: {_room.GroupName}, No: {_clientId}, Username: {_username}, Context Id: {Context.ContextId})");

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

            // var memberCount = await _room.GetMemberCountAsync();
            // Console.WriteLine($"[ChatStreamingHub] LeaveAsync | Member count: {memberCount}");

            BroadcastToSelf(_room).OnLeave(response);
            BroadcastExceptSelf(_room).OnUserLeave(response);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_clientId < 0) { return; }

            // Console.WriteLine($"[ChatStreaming.SendMessage] Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            var response = new MessageResponse()
            {
                Username = _username,
                Message = message
            };

            Broadcast(_room).OnReceiveMessage(response);
        }

        /// <summary>
        /// Handle connection if needed.
        /// </summary>
        /// <returns></returns>
        protected override ValueTask OnConnecting()
        {
            Console.WriteLine($"[ChatStreamingHub] OnConnecting | Context Id: {Context.ContextId}");
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

            // Console.WriteLine($"[ChatStreamingHub] OnDisconnected | Room: {_room.GroupName}, ClientId: {_clientId}, Username: {_username}, ContextId: {Context.ContextId})");

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