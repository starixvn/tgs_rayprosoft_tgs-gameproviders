using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Services.Joker;

internal class JokerGamePlayer : IJokerGamePlayer
{
    private readonly IJokerApiClient _client;

    public JokerGamePlayer(
        IJokerApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<bool> CreatePlayerAsync(string playerCode, Uri apiUrl, JokerAgent agent)
        => _client.CreateUserAsync(apiUrl, playerCode, agent.AgentId, agent.SecretKey);
}
