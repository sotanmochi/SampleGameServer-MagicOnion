using System;
using System.Threading;
using System.Threading.Tasks;
using MagicOnion.Client;
using GameServer.Shared.MessagePackObject;
using GameServer.Shared.Streaming;

namespace GameServer.UnityClient
{
    public sealed class GameStreamingClient : StreamingClientBase<IGameHub, IGameHubReceiver>, IGameHubReceiver
    {
        public event Action<JoinResponse> OnJoin;
        public event Action<JoinResponse> OnLeave;
        public event Action<JoinResponse> OnUserJoin;
        public event Action<JoinResponse> OnUserLeave;
        public event Action<PlayerPoseObject> OnReceivePlayerPose;

        public async Task Join(string roomId, string username)
        {
            if (!Connected)
            {
                DebugLogger.Log($"<color=orange>[GameStreamingClient] Join | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[GameStreamingClient] Join | RoomId: {roomId}, Username: {username}");
            await _streamingClient.JoinAsync(new JoinRequest(){ RoomId = roomId, Username = username });
        }

        public async Task Leave()
        {
            if (!Connected)
            {
                DebugLogger.Log($"<color=orange>[GameStreamingClient] Leave | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[GameStreamingClient] Leave");
            await _streamingClient.LeaveAsync();
        }

        public void SendPlayerPose(PlayerPoseObject value)
        {
            if (!Connected)
            {
                DebugLogger.Log($"<color=orange>[GameStreamingClient] SendEvent | This client has already been disconnected.</color>");
                return;
            }

            DebugLogger.Log($"[GameStreamingClient] SendEvent | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            _streamingClient.SendPlayerPoseAsync(value);
        }

        protected override async Task ConnectClientAsync()
        {
            DebugLogger.Log($"<color=orange>[GameStreamingClient] ConnectClientAsync | Thread Id: {Thread.CurrentThread.ManagedThreadId}</color>");
            _streamingClient = await StreamingHubClient.ConnectAsync<IGameHub, IGameHubReceiver>(_channel, this, 
                                                        cancellationToken: _shutdownCancellation.Token);
        }

        void IGameHubReceiver.OnReceivePlayerPose(PlayerPoseObject value)
        {
            DebugLogger.Log($"[GameStreamingClient] OnReceivePlayerPose | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnReceivePlayerPose?.Invoke(value);
        }

        void IGameHubReceiver.OnJoin(JoinResponse response)
        {
            DebugLogger.Log($"[GameStreamingClient] OnJoin | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnJoin?.Invoke(response);
        }

        void IGameHubReceiver.OnLeave(JoinResponse response)
        {
            DebugLogger.Log($"[GameStreamingClient] OnLeave | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            OnLeave?.Invoke(response);
        }

        void IGameHubReceiver.OnUserJoin(JoinResponse response)
        {
            DebugLogger.Log($"[GameStreamingClient] OnUserJoin | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            DebugLogger.Log($"[GameStreamingClient] OnUserJoin | Username: {response.Username}");
            OnUserJoin?.Invoke(response);
        }

        void IGameHubReceiver.OnUserLeave(JoinResponse response)
        {
            DebugLogger.Log($"[GameStreamingClient] OnUserLeave | Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            DebugLogger.Log($"[GameStreamingClient] OnUserLeave | Username: {response.Username}");
            OnUserLeave?.Invoke(response);
        }
    }
}