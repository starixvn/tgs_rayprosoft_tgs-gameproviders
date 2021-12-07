using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Player;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerModel?> GetAsync(string playerCode);

        Task<decimal?> GetPlayerBalanceAsync(string playerCode);
    }
}
