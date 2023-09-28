using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Card365.ApplicationServices;

internal class Card365RoundDetailGetService : IGetService<Card365RoundDetailQuery, RoundDetailResult>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IRoundRepository _roundService;

    public Card365RoundDetailGetService(
        IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = false };
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        Card365RoundDetailQuery query,
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

    private string GetGameHistory(string roundDetail)
    {
        if (string.IsNullOrWhiteSpace(roundDetail))
        {
            return string.Empty;
        }

        var items = JsonSerializer.Deserialize<Dictionary<string, object>>(roundDetail);
        if (items == null)
        {
            return string.Empty;
        }

        var tiles = items.TryGetValue("Tiles", out var value) && value != null ? value.ToString() : string.Empty;
        var isBanker = items.TryGetValue("Banker", out value) && "0".Equals(value!.ToString());

        return JsonSerializer.Serialize(new
        {
            IsBanker = isBanker,
            Tiles = tiles
        }, _jsonSerializerOptions);
    }
}
