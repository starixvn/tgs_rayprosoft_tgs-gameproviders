using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface IPlayerService
    {
        Task<PlayerInfo> GetAsync(string playerCode);

        Task<PlayerInfo> GetCacheAsync(string playerCode);
    }
}
