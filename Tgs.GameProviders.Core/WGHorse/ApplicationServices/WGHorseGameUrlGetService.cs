using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices;

public class WGHorseGameUrlGetService : IGetService<WGHorseGameUrlQuery, GameUrlResult>
{
    private readonly IWGHorseApiClient _client;
    private readonly ILogger<WGHorseGameUrlGetService> _logger;
    private readonly IWGHorseService _service;

    public WGHorseGameUrlGetService(
        ILogger<WGHorseGameUrlGetService> logger,
        IWGHorseApiClient client,
        IWGHorseService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<GameUrlResult> GetAsync(WGHorseGameUrlQuery command, CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var cultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
        var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);

        var token = await _client.GetPrepareLoginAsync(
            configuration.ApiUrl,
            configuration.PartnerCode,
            configuration.PartnerKey,
            configuration.IV,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            command.PlayerCode,
            command.PlayerCode,
            currencyCode).ConfigureAwait(false);

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var gameUrl = await _client.GetLoginAsync(
            configuration.ApiUrl,
            configuration.PartnerCode,
            token,
            cultureCode).ConfigureAwait(false);

        return gameUrl != null ? (new() { Url = gameUrl }) : null;
    }
}
