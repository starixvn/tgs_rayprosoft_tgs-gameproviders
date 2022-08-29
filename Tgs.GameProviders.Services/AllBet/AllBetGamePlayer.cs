using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Core.AllBet.Models;
using Tgs.GameProviders.Domains.SettingConfiguration.AllBet;

namespace Tgs.GameProviders.Services.AllBet;

internal class AllBetGamePlayer : IAllBetGamePlayer
{
    private readonly IAllBetApiClient _client;
    private readonly ILogger<AllBetGamePlayer> _logger;

    public AllBetGamePlayer(
        ILogger<AllBetGamePlayer> logger,
        IAllBetApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<bool> CheckOrCreateMemberAsync(string allBetPlayerId, Uri apiUrl, AllBetAgent agent)
    {
        var request = new AllBetCheckOrCreateMemberRequest
        {
            AgentId = agent.AgentId,
            PlayerId = allBetPlayerId
        };

        var response = await _client
            .CheckOrCreateMemberAsync(apiUrl, agent.PropertyId, agent.AllBetApiKey, request)
            .ConfigureAwait(false);

        return response.ErrorCode is nameof(AllBetApiErrorCode.OK) or nameof(AllBetApiErrorCode.PLAYER_EXIST);
    }
}
