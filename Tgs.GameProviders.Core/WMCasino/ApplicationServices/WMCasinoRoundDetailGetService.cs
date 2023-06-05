using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasino.ApplicationServices;

internal class WMCasinoRoundDetailGetService : IGetService<WMCasinoRoundDetailQuery, RoundDetailResult>
{
    private readonly ILogger<WMCasinoRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundService;

    public WMCasinoRoundDetailGetService(
        ILogger<WMCasinoRoundDetailGetService> logger,
        IRoundRepository roundService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        WMCasinoRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var value = GetGameHistory(roundInfo.AdditionalData);

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
