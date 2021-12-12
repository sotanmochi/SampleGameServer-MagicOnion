using System.Threading.Tasks;
using MagicOnion;
using GameServer.Shared.MessagePackObject;

namespace GameServer.Shared.Streaming
{
    /// <summary>
    /// Client -> Server API (Streaming)
    /// </summary>
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        Task JoinAsync(JoinRequest request);
        Task LeaveAsync();
        Task SendMessageAsync(string message);
    }
}
