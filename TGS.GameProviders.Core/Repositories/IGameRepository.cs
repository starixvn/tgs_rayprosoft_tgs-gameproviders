using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IGameRepository
    {
        Task<GetGame> GetGameAsync(int gameId);

        Task<GetGame> GetGameAsync(Guid gameProviderId, string externalGameId);

        Task<GetGame[]> GetGameList(Guid gameProviderId);

        Task<GetGame[]> GetGameList(Guid gameProviderId, GameType gameType);
    }
}
