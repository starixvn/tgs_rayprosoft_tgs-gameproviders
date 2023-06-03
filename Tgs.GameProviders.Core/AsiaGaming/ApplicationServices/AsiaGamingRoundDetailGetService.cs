using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.AsiaGaming.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration.AsiaGaming;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AsiaGaming.ApplicationServices;

internal class AsiaGamingRoundDetailGetService : IGetService<AsiaGamingRoundDetailQuery, RoundDetailResult>
{
    private readonly IGameRepository _gameService;
    private readonly ILogger<AsiaGamingRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundService;
    private readonly IAsiaGamingService _service;

    public AsiaGamingRoundDetailGetService(
        ILogger<AsiaGamingRoundDetailGetService> logger,
        IRoundRepository roundService,
        IGameRepository gameService,
        IAsiaGamingService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        AsiaGamingRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var gameInfo = await _gameService
            .GetAsync(
                roundInfo.GameId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agGameType = gameInfo.GameExternalId.Split('_')[2];

        var gameHistoryType = configuration.GameHistoryTypes?
            .FirstOrDefault(h => h.GameTypes.Contains(agGameType, StringComparer.OrdinalIgnoreCase));

        var value = GetGameHistory(gameHistoryType, roundInfo.AdditionalData);

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

    private string GetCard(string cardCode)
    {
        if (cardCode.Length < 2)
        {
            return cardCode;
        }

        var agCode = cardCode[0].ToString();
        var code = agCode switch
        {
            "C" => "♣",
            "D" => "♦",
            "H" => "♥",
            _ => "♠"
        };

        return cardCode.Replace(agCode, code);
    }

    private string GetCards(IEnumerable<string> cards)
    {
        var items = cards.Select(GetCard);

        return string.Join(';', items);
    }

    private string GetGameHistory(AsiaGamingGameHistoryType gameHistoryType, string roundDetail)
    {
        if (gameHistoryType == null)
        {
            return roundDetail;
        }

        var gameHistory = JsonSerializer.Deserialize<AsiaGamingGameHistory>(roundDetail)!;

        var results = gameHistory.RoundResult.Split(':');

        var historyData = new Dictionary<string, string>();

        foreach (var result in results)
        {
            var dataSplitBySemicolon = result.Split(';');
            var type = dataSplitBySemicolon[0];
            var cards = GetCards(dataSplitBySemicolon.Skip(1));
            historyData.Add(gameHistoryType.Descriptions[type], cards);
        }

        return JsonSerializer.Serialize(historyData);
    }
}
