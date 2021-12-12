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
        private int _clientNumber;

        public async Task JoinAsync(JoinRequest request)
        {
            Console.WriteLine($"[ChatStreamingHub] JoinAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            _room = await Group.AddAsync(request.RoomId);
            _username = request.Username;
            _clientNumber = await _room.GetMemberCountAsync();

            Console.WriteLine($"[ChatStreamingHub] JoinAsync | Room: {_room.GroupName}, Username: {_username}, Context Id: {Context.ContextId})");
            Console.WriteLine($"[ChatStreamingHub] JoinAsync | Member count: {_clientNumber}");

            var response = new JoinResponse()
            {
                ClientNumber = (uint)_clientNumber,
                ClientConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            BroadcastToSelf(_room).OnJoin(response);
            BroadcastExceptSelf(_room).OnUserJoin(response);
        }

        public async Task LeaveAsync()
        {
            Console.WriteLine($"[ChatStreamingHub] LeaveAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"[ChatStreamingHub] LeaveAsync | Room: {_room.GroupName}, No: {_clientNumber}, Username: {_username}, Context Id: {Context.ContextId})");

            await _room.RemoveAsync(Context);

            var memberCount = await _room.GetMemberCountAsync();
            Console.WriteLine($"[ChatStreamingHub] LeaveAsync | Member count: {memberCount}");

            var response = new JoinResponse()
            {
                ClientNumber = (uint)_clientNumber,
                ClientConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            BroadcastToSelf(_room).OnLeave(response);
            BroadcastExceptSelf(_room).OnUserLeave(response);
        }

        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine($"[ChatStreaming.SendMessage] Thread Id: {Thread.CurrentThread.ManagedThreadId}");

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
            Console.WriteLine($"[ChatStreamingHub] OnDisconnected | Room: {_room.GroupName}, No: {_clientNumber}, Username: {_username}, Context Id: {Context.ContextId})");

            var response = new JoinResponse()
            {
                ClientNumber = (uint)_clientNumber,
                ClientConnectionId = Context.ContextId.ToString(),
                RoomId = _room.GroupName,
                Username = _username,
            };

            BroadcastExceptSelf(_room).OnUserLeave(response);

            return CompletedTask;
        }
    }
}