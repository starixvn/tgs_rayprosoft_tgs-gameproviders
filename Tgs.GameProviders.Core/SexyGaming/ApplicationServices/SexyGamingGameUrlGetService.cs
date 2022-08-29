using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SexyGaming.ApplicationServices;

public class SexyGamingGameUrlGetService : IGetService<SexyGamingGameUrlQuery, GameUrlResult>
{
    private readonly ISexyGamingApiClient _client;
    private readonly ISexyGamingGamePlayer _gamingGamePlayerCreate;
    private readonly ILogger<SexyGamingGameUrlGetService> _logger;
    private readonly ISexyGamingService _service;
    private readonly ITokenProvider _tokenProvider;

    public SexyGamingGameUrlGetService(
        ISexyGamingApiClient client,
        ILogger<SexyGamingGameUrlGetService> logger,
        ISexyGamingService service,
        ITokenProvider tokenProvider,
        ISexyGamingGamePlayer gamingGamePlayerCreate
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _gamingGamePlayerCreate = gamingGamePlayerCreate ?? throw new ArgumentNullException(nameof(gamingGamePlayerCreate));
    }

    public async ValueTask<GameUrlResult> GetAsync(SexyGamingGameUrlQuery command, CancellationToken cancellationToken = default)
    {
        var encryptedToken = await _tokenProvider.EncryptAsync(command.LoginRecordId).ConfigureAwait(false);
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var gameProviderCulture = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
        var gameProviderCurrency = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);

        if (configuration == null)
        {
            return null;
        }

        var data = new SexyGamingLoginGameRequest
        {
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            UserId = command.PlayerCode,
            IsMobileLogin = command.PlatformType == PlatformType.Mobile,
            Language = gameProviderCulture,
            Extension1 = JsonSerializer.Serialize(new
            {
                token = encryptedToken
            })
        };

        var responseGameUrl = await _client.GetGamingLoginGameAsync(configuration.ApiUrl, data).ConfigureAwait(false);

        if (!_service.IsSuccess(responseGameUrl.Status))
        {
            if (_service.IsAccountIsNotExists(responseGameUrl.Status))
            {
                if (await _gamingGamePlayerCreate.CreatePlayerAsync(configuration, command.PlayerCode, command.CurrencyCode, gameProviderCurrency, gameProviderCulture))
                {
                    responseGameUrl = await _client.GetGamingLoginGameAsync(configuration.ApiUrl, data).ConfigureAwait(false);
                }
            }
        }
        return _service.IsSuccess(responseGameUrl.Status) ? new() { Url = new Uri(responseGameUrl.Url) } : null;
    }
}
