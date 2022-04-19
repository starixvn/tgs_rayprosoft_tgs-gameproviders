using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerGameUrlGetService : IGetService<JokerGameUrlQuery, GameUrlResult>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IJokerService _service;

    public JokerGameUrlGetService(
        IJokerApiClient client,
        IJokerAgentMapper agentMapper,
        IJokerService service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        JokerGameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(configuration!.BrandMerchants, query.BrandCode, query.CurrencyCode);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{query.BrandCode}' and currency '{query.CurrencyCode}'");
        }

        var gameToken = await _client.GetGameTokenAsync(
                configuration.ApiUrl,
                query.PlayerCode,
                agent.AgentId,
                agent.SecretKey)
            .ConfigureAwait(false);

        if (gameToken == null)
        {
            return null;
        }

        var game = await _service.GetGameAsync(query.GameId!.Value).ConfigureAwait(false);

        var cultureCode = await _service.GetGameProviderLanguageCodeAsync(query.Language).ConfigureAwait(false);

        var queryString = new Dictionary<string, string>
        {
            ["token"] = gameToken,
            ["game"] = game.GameExternalId,
            ["mobile"] = (query.PlatformType == PlatformType.Mobile).ToString().ToLower(),
            ["lang"] = cultureCode
        };

        var url = QueryHelpers.AddQueryString(configuration.GameLaunchUrl.OriginalString, queryString);

        return new GameUrlResult
        {
            Url = new Uri(url)
        };
    }
}