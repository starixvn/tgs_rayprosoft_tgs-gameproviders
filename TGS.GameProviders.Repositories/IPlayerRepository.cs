using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    public interface IPlayerRepository
    {
        Task<GetPlayer?> GetPlayerAsync(string playerCode);

        Task<Player?> GetPlayerAsync(Guid brandId, string username);
    }
}
