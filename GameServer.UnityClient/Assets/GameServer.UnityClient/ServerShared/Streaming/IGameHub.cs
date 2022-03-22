using System.Threading.Tasks;
using MagicOnion;
using GameServer.Shared.MessagePackObject;

namespace GameServer.Shared.Streaming
{
    /// <summary>
    /// Client -> Server API (Streaming)
    /// </summary>
    public interface IGameHub : IStreamingHub<IGameHub, IGameHubReceiver>
    {
        Task SendPlayerPoseAsync(PlayerPoseObject value);
        Task JoinAsync(JoinRequest request);
        Task LeaveAsync();
    }
}
