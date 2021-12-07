using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration.AsiaGaming;

namespace TGS.GameProviders.Core.AsiaGaming.ApplicationServices;

internal class AsiaGamingRoundDetailGetService : IGetService<AsiaGamingRoundDetailQuery, RoundDetailResult>
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<AsiaGamingRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundRepository;
    private readonly IAsiaGamingService _service;

    public AsiaGamingRoundDetailGetService(
        ILogger<AsiaGamingRoundDetailGetService> logger,
        IRoundRepository roundRepository,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        IAsiaGamingService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        AsiaGamingRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundModel == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);
        if (gameProviderModel == null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
        }

        var gameModel = await _gameRepository.GetAsync(roundModel.GameId).ConfigureAwait(false);
        if (gameModel == null)
        {
            throw new Exception($"This game Id could not be found.{roundModel.GameId}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agGameType = gameModel.GameExternalId.Split('_')[2];

        var gameHistoryType = configuration.GameHistoryTypes?
            .FirstOrDefault(h => h.GameTypes.Contains(agGameType, StringComparer.OrdinalIgnoreCase));

        var value = GetGameHistory(gameHistoryType, roundModel.AdditionalData);

        return new RoundDetailResult
        {
            GameProviderName = gameProviderModel.Name,
            GameName = gameModel.Name,
            ExternalRoundId = roundModel.ExternalRoundId,
            BetAmount = roundModel.BetAmount,
            Turnover = roundModel.Turnover,
            Winloss = roundModel.WinLoss,
            GameResult = roundModel.ClientStatus.ToString(),
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