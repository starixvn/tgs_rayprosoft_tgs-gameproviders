using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    public interface IGameRepository
    {
        Task<Game> GetGameAsync(int gameId);

        Task<Game> GetGameAsync(Guid gameProviderId, string externalGameId);
    }
}
