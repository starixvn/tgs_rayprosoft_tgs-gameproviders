using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IGameProvidersQueryServerClient _client;

        public PlayerRepository(IGameProvidersQueryServerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<PlayerModel> GetAsync(string playerCode)
           => await _client.PlayerGetAsync(playerCode).ConfigureAwait(false) ?? throw new Exception($"The player does not exist.{playerCode}");
    }
}
