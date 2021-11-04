using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Player;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IPlayerRepository
    {
        Task<GetPlayer?> GetPlayerAsync(string playerCode);

        Task<GetPlayer?> GetPlayerAsync(Guid brandId, string username);

        Task<decimal?> GetPlayerBalanceAsync(string playerCode);
    }
}