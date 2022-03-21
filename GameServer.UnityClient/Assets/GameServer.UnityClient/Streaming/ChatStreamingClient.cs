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
        public event Action<JoinResponse> OnJoin;
        public event Action<JoinResponse> OnLeave;
        public event Action<JoinResponse> OnUserJoin;
        public event Action<JoinResponse> OnUserLeave;
        public event Action<MessageResponse> OnReceiveMessage;

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

            DebugLogger.Log($"[ChatStreamingClient] SendMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _streamingClient.SendMessageAsync(message);
        }

        protected override async Task ConnectClientAsync()
        {
            DebugLogger.Log($"<color=orange>[ChatStreamingClient] ConnectClientAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}</color>");
            _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(_channel, this, 
                                                        cancellationToken: _shutdownCancellation.Token);
        }

        void IChatHubReceiver.OnReceiveMessage(MessageResponse message)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnReceiveMessage | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnReceiveMessage?.Invoke(message);
        }

        void IChatHubReceiver.OnJoin(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnJoin | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnJoin?.Invoke(response);
        }

        void IChatHubReceiver.OnLeave(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnLeave | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnLeave?.Invoke(response);
        }

        void IChatHubReceiver.OnUserJoin(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnUserJoin | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            DebugLogger.Log($"[ChatStreamingClient] OnUserJoin | Username: {response.Username}");
            OnUserJoin?.Invoke(response);
        }

        void IChatHubReceiver.OnUserLeave(JoinResponse response)
        {
            DebugLogger.Log($"[ChatStreamingClient] OnUserLeave | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            DebugLogger.Log($"[ChatStreamingClient] OnUserLeave | Username: {response.Username}");
            OnUserLeave?.Invoke(response);
        }
    }
}