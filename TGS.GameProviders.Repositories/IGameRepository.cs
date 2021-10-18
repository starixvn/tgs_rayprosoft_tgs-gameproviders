using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;

namespace TGS.GameProviders.Repositories
{
    public interface IGameRepository
    {
        Task<GetGame> GetGameAsync(int gameId);

        Task<GetGame> GetGameAsync(Guid gameProviderId, string externalGameId);

        Task<GetGame[]> GetGameList(Guid gameProviderId);

        Task<GetGame[]> GetGameList(Guid gameProviderId, GameType gameType);
    }
}
