#nullable enable

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferRoundDetailGetService
    : IGetService<PragmaticPlayTransferRoundDetailQuery, RoundDetailResult?>
{
    private readonly IPragmaticPlayTransferApiClient _client;
    private readonly IGameService _gameService;
    private readonly IRoundService _roundService;
    private readonly IPragmaticPlayTransferService _service;

    public PragmaticPlayTransferRoundDetailGetService(
        IRoundService roundService,
        IGameService gameService,
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult?> GetAsync(
        PragmaticPlayTransferRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo is null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var gameProviderInfo = await _service.GetGameProviderAsync(roundInfo.GameProviderId).ConfigureAwait(false);
        if (gameProviderInfo is null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundInfo.GameProviderId}");
        }

        var gameInfo = await _gameService.GetCacheAsync(roundInfo.GameId).ConfigureAwait(false);
        if (gameInfo is null)
        {
            throw new Exception($"This game does not exist.{roundInfo.GameId}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration is null)
        {
            throw new Exception($"The {GameProviderType.PragmaticPlayTransfer} configuration does not exist.");
        }

        var gameProviderCultureCode = await _service
            .GetGameProviderLanguageCodeAsync(gameProviderInfo.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var url = await _client
            .GetGameDetailUrlAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                roundInfo.PlayerCode,
                roundInfo.ExternalRoundId,
                gameInfo.GameExternalId,
                gameProviderCultureCode)
            .ConfigureAwait(false);

        var jackpot = 0m;
        if (!string.IsNullOrEmpty(roundInfo.AdditionalData))
        {
            var record = JsonSerializer.Deserialize<PragmaticPlayTransferGameRecordData>(roundInfo.AdditionalData);
            jackpot = record?.Jackpot ?? 0m;
        }

        var result = new
        {
            Url = url ?? string.Empty,
            Jackpot = jackpot
        };

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson
                {
                    Value = JsonSerializer.Serialize(result)
                }
            }
        };
    }
}
