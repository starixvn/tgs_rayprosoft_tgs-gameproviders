using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferGameUrlGetService : IGetService<PragmaticPlayTransferGameUrlQuery, GameUrlResult>
{
    private readonly IPragmaticPlayTransferService _service;
    private readonly IPragmaticPlayTransferApiClient _client;

    public PragmaticPlayTransferGameUrlGetService(
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        PragmaticPlayTransferGameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration is null)
            return null;

        var game = await _service.GetGameAsync(query.GameId!.Value).ConfigureAwait(false);
        if (game is null)
            return null;

        var culture = await _service.GetGameProviderLanguageCodeAsync(game.GameProviderId, query.Language).ConfigureAwait(false);

        var platform = query.PlatformType == PlatformType.Mobile ? "MOBILE" : "WEB";

        var url = await _client
            .GetGameUrlAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                query.PlayerCode,
                game.GameExternalId,
                culture,
                platform,
                query.BackUrl?.OriginalString)
            .ConfigureAwait(false);

        return url != null
            ? new GameUrlResult
            {
                Url = new Uri(url)
            }
            : null;
    }
}