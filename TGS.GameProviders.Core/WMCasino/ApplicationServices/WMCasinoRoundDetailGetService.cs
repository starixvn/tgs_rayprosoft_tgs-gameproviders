using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.WMCasino.ApplicationServices;

internal class WMCasinoRoundDetailGetService : IGetService<WMCasinoRoundDetailQuery, RoundDetailResult>
{
    private readonly ILogger<WMCasinoRoundDetailGetService> _logger;
    private readonly IRoundService _roundService;
    private readonly IWMCasinoService _service;

    public WMCasinoRoundDetailGetService(
        ILogger<WMCasinoRoundDetailGetService> logger,
        IRoundService roundService,
        IWMCasinoService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        WMCasinoRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundModel == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var gameProviderModel = await _service.GetGameProviderAsync(roundModel.GameProviderId).ConfigureAwait(false);
        if (gameProviderModel == null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
        }

        var gameModel = await _service.GetGameAsync(roundModel.GameId).ConfigureAwait(false);
        if (gameModel == null)
        {
            throw new Exception($"This game Id could not be found.{roundModel.GameId}");
        }

        var value = GetGameHistory(roundModel.AdditionalData);

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson
                {
                    Value = value
                }
            }
        };
    }

    private string GetGameHistory(string roundDetail)
    {
        if (string.IsNullOrWhiteSpace(roundDetail))
        {
            return string.Empty;
        }

        var results = roundDetail.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        if (results.Length == 0)
        {
            return string.Empty;
        }

        if (results.Length == 1)
        {
            return results[0];
        }

        var items = results
            .Select(r => r.Split(":", StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(r => r[0], r => r[1]);

        return items.Count == 0 ? string.Empty : JsonSerializer.Serialize(items);
    }
}
