using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.Player;

namespace TGS.GameProviders.Repositories
{
    public interface IPlayerRepository
    {
        Task<GetPlayer?> GetPlayerAsync(string playerCode);

        Task<GetPlayer?> GetPlayerAsync(Guid brandId, string username);
    }
}
