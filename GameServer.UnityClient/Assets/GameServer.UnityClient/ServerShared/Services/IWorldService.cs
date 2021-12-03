using System.Collections.Generic;
using MagicOnion;

namespace GameServer.Shared.Services
{
    public interface IWorldService : IService<IWorldService>
    {
        UnaryResult<List<string>> FindWorldIdOrderByRank();
        UnaryResult<World> FindWorld(string worldId);
    }
}