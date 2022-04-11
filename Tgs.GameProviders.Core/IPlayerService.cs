using System.Threading.Tasks;

namespace Tgs.GameProviders.Core
{
    public interface IPlayerService
    {
        Task<PlayerInfo> GetAsync(string playerCode);

        Task<PlayerInfo> GetCacheAsync(string playerCode);
    }
}
