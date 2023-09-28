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
    private readonly IRoundRepository _roundService;

    public WMCasinoRoundDetailGetService(
        IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        WMCasinoRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

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

    private static string GetGameHistory(string roundDetail)
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
