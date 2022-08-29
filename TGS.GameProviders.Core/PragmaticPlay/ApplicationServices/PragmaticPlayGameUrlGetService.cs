using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.Core.PragmaticPlay.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlay.ApplicationServices;

internal class PragmaticPlayGameUrlGetService : IGetService<PragmaticPlayGameUrlQuery, GameUrlResult>
{
    private readonly IPragmaticPlayApiClient _client;
    private readonly ILogger<PragmaticPlayGameUrlGetService> _logger;
    private readonly IPragmaticPlayService _service;
    private readonly ITokenProvider _tokenProvider;

    public PragmaticPlayGameUrlGetService(
        ILogger<PragmaticPlayGameUrlGetService> logger,
        ITokenProvider tokenProvider,
        IPragmaticPlayService service,
        IPragmaticPlayApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        PragmaticPlayGameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var encryptedToken = await _tokenProvider.EncryptAsync(query.LoginRecordId).ConfigureAwait(false);

        var game = await _service.GetGameAsync(query.GameId!.Value).ConfigureAwait(false);
        var culture = await _service.GetGameProviderLanguageAsync(game.GameProviderId, query.Language).ConfigureAwait(false);

        var request = new PragmaticPlayGameUrlRequest
        {
            SecureLogin = configuration.SecureLogin,
            Token = encryptedToken,
            ExternalGameId = game.GameExternalId,
            Language = culture,
            Platform = query.PlatformType == PlatformType.Mobile ? "MOBILE" : "WEB",
            LobbyUrl = query.BackUrl?.AbsoluteUri
        };

        var response = await _client.GetGameUrlAsync(configuration.ApiUrl, configuration.SecretKey, request)
            .ConfigureAwait(false);

        if (response.Error != "0")
        {
            return null;
        }

        return new GameUrlResult
        {
            Url = new Uri(response.Url)
        };
    }
}
