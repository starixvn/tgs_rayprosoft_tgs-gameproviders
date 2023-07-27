using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

public class DreamGameGameUrlGetService : IGetService<DreamGameGameUrlQuery, GameUrlResult>
{
    private readonly IGameInterfaceDreamGamingClient _client;
    private readonly IDreamGameService _service;

    public DreamGameGameUrlGetService(
        IDreamGameService service,
        IGameInterfaceDreamGamingClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(DreamGameGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _service.GetGameProviderLanguageCodeAsync(query.Language).ConfigureAwait(false);
        var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).ConfigureAwait(false);
        var tableLimit = configuration.BetLimits.Where(x => string.Equals(x.Currency, query.CurrencyCode, StringComparison.OrdinalIgnoreCase)).Single().TableLimit;
        if (string.IsNullOrEmpty(tableLimit))
        {
            throw new Exception($"Currency limit red does not exist.({query.CurrencyCode})");
        }

        var response = await _client
            .GameUrlAsync(
                apiUrl: configuration.ApiUrl,
                apiKey: configuration.ApiKey,
                agentName: configuration.AgentName,
                username: query.PlayerCode,
                password: configuration.PlayerPassword,
                isMobile: query.PlatformType == PlatformType.Mobile,
                lang: gameProviderCultureCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new() { Url = new(response.GameUrl) };
    }
}
