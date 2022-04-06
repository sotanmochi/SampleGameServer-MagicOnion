using System;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using GameServer.Shared.MessagePackObject;
using GameServer.Shared.Streaming;

namespace GameServer.Streaming
{
    /// <summary>
    /// Chat server processing. One class instance for one connection.
    /// </summary>
    public sealed class ChatStreamingHub : StreamingHubBase<IChatHub, IChatHubReceiver>, IChatHub
    {
        private IGroup _room;
        private int _clientId = -1;
        private string _username = "";

        public async Task JoinAsync(JoinRequest request)
        {
            var roomId = $"ChatStreaming_{request.RoomId}";

            if (_clientId >= 0 && roomId == _room?.GroupName) { return; }

            _room = await Group.AddAsync(roomId);

            var response = new JoinResponse()
            {
                ClientId = -1,
                ConnectionId = Context.ContextId.ToString(),
                RoomId = roomId,
                Username = request.Username,
            };

            var newClientId = ClientIdPoolStorage.GetClientId(roomId);
            if (newClientId < 0)
            {
                BroadcastToSelf(_room).OnLeave(response);
                await _room.RemoveAsync(Context);
                return;
            }

            _clientId = newClientId;
            _username = request.Username;

            response.ClientId = _clientId;
            response.Username = _username;

            BroadcastToSelf(_room).OnJoin(response);
            BroadcastExceptSelf(_room).OnUserJoin(response);
        }

        public async Task LeaveAsync()
        {
            if (_clientId < 0) { return; }

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
            _username = "";

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