using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Habanero.ApplicationServices;

internal class HabaneroRoundDetailGetService : IGetService<HabaneroRoundDetailQuery, RoundDetailResult>
{
    private readonly ILogger<HabaneroRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundService;
    private readonly IHabaneroService _service;

    public HabaneroRoundDetailGetService(
        ILogger<HabaneroRoundDetailGetService> logger,
        IHabaneroService service,
        IRoundRepository roundService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        HabaneroRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderModel = await _service
            .GetGameProviderAsync(
            roundInfo.GameProviderId)
            .ConfigureAwait(false);

        var configuration = await _service
            .GetSettingConfigurationAsync()
            .ConfigureAwait(false);

        var gameProviderCulture = await _service
            .GetGameProviderLanguageCodeAsync(gameProviderModel.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var valueToHash =
            $"{roundInfo.ExternalRoundId.ToLower()}{configuration.BrandId.ToLower()}{configuration.ApiKey.ToLower()}";

        var hashKey = Sha256Hash(valueToHash);

        var url = $"{configuration.BetHistoryUrl.AbsoluteUri}games/history"
            .AppendQueryString($"brandId={configuration.BrandId}")
            .AppendQueryString($"friendlyid={roundInfo.ExternalRoundId}")
            .AppendQueryString("viewtype=game")
            .AppendQueryString($"local={gameProviderCulture}")
            .AppendQueryString($"hash={hashKey}");

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = url
                }
            }
        };
    }

    private string Sha256Hash(string value)
        => new BigInteger(SHA256.HashData(Encoding.UTF8.GetBytes(value)), isBigEndian: true)
            .ToString("x");
}
