using System;
using System.Threading;
using System.Threading.Tasks;
using MagicOnion.Client;
using GameServer.Shared.MessagePackObject;
using GameServer.Shared.Streaming;

namespace GameServer.UnityClient
{
    public sealed class ChatStreamingClient : StreamingClientBase<IChatHub, IChatHubReceiver>, IChatHubReceiver
    {
        public event Action<(string RoomId, string Username)> OnJoin;
        public event Action<(string RoomId, string Username)> OnLeave;
        public event Action<(string RoomId, string Username)> OnUserJoin;
        public event Action<(string RoomId, string Username)> OnUserLeave;
        public event Action<(string Username, string Message)> OnReceiveMessage;

        public async Task Join(string roomId, string username)
        {
            if (!Connected)
            {
                DebugLogger.Log($"<color=orange>[ChatStreamingClient] Join | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[ChatStreamingClient] Join | RoomId: {roomId}, Username: {username}");
            await _streamingClient.JoinAsync(new JoinRequest(){ RoomId = roomId, Username = username });
        }

        public async Task Leave()
        {
            if (!Connected)
            {
                DebugLogger.Log($"<color=orange>[ChatStreamingClient] Leave | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[ChatStreamingClient] Leave");
            await _streamingClient.LeaveAsync();
        }

        public void SendMessage(string message)
        {
            if (!Connected)
            {
                DebugLogger.Log($"<color=orange>[ChatStreamingClient] SendMessage | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[ChatStreamingClient] SendMesage");
            _streamingClient.SendMessageAsync(message);
        }

        protected override async Task ConnectClientAsync()
        {
            DebugLogger.Log($"[ChatStreamingClient] ConnectClientAsync");
            _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(_channel, this, 
                                                        cancellationToken: _shutdownCancellation.Token);
        }

        void IChatHubReceiver.OnReceiveMessage(MessageResponse message)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnReceiveMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnReceiveMessage?.Invoke((message.Username, message.Message));
        }

        void IChatHubReceiver.OnJoin(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnJoin | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnJoin?.Invoke((response.RoomId, response.Username));
        }

        void IChatHubReceiver.OnLeave(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnLeave | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnLeave?.Invoke((response.RoomId, response.Username));
        }

        void IChatHubReceiver.OnUserJoin(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnUserJoin | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            DebugLogger.Log($"[ChatStreamingClient] OnUserJoin | Username: {response.Username}");
            OnUserJoin?.Invoke((response.RoomId, response.Username));
        }

        void IChatHubReceiver.OnUserLeave(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnUserLeave | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            DebugLogger.Log($"[ChatStreamingClient] OnUserLeave | Username: {response.Username}");
            OnUserLeave?.Invoke((response.RoomId, response.Username));
        }
    }
}