using MagicOnion;

namespace GameServer.Shared.Services.Admin
{
    public interface IWorldAdminService : IService<IWorldAdminService>
    {
        UnaryResult<bool> CreateOrUpdateWorld(World world);
    }
}