using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.AllBet.Models;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.AllBet.ApplicationServices;

internal class AllBetRoundDetailGetService : IGetService<AllBetRoundDetailQuery, RoundDetailResult>
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameService _gameService;
    private readonly ILogger<AllBetRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundRepository;

    public AllBetRoundDetailGetService(
        ILogger<AllBetRoundDetailGetService> logger,
        IRoundRepository roundRepository,
        IGameProviderRepository gameProviderRepository,
        IGameService gameService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        AllBetRoundDetailQuery query,
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

        var gameModel = await _gameService.GetCacheAsync(roundModel.GameId).ConfigureAwait(false);
        if (gameModel == null)
        {
            throw new Exception($"This game Id could not be found.{roundModel.GameId}");
        }

        var value = GetGameHistory(roundModel.AdditionalData);

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

    private string GetBaccaratGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);

        var results = new Dictionary<string, string>();

        for (var i = 0; i < values.Length; i++)
        {
            var key = i == 0 ? "Player" : "Banker";
            var details = values[i].SplitByComma();
            var cards = GetCards(details);

            results[key] = cards;
        }

        return JsonSerializer.Serialize(results);
    }

    private string GetBullBullGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);

        var results = new Dictionary<string, string>();

        for (var i = 0; i < values.Length; i++)
        {
            var key = i switch
            {
                0 => "First",
                1 => "Banker",
                _ => $"Player{i - 1}"
            };

            var details = values[i].Split(',');
            var cards = GetCards(details);

            results[key] = cards;
        }

        return JsonSerializer.Serialize(results);
    }

    private string GetCard(char code) => code switch
    {
        '2' => "♥",
        '3' => "♣",
        '4' => "♦",
        _ => "♠"
    };

    private string GetCards(string[] details)
    {
        var cards = details.Where(v => v != "-1")
            .Select(v => $"{GetCard(v[0])}{GetNumber(v[1..])}");

        return string.Join(";", cards);
    }

    private string GetDragonTigerGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);

        var results = new Dictionary<string, string>();

        for (var i = 0; i < values.Length; i++)
        {
            var key = i == 0 ? "Dragon" : "Tiger";
            var cards = GetCards(new[]
            {
                values[i]
            });

            results[key] = cards;
        }

        return JsonSerializer.Serialize(results);
    }

    private string GetGameHistory(string roundDetail)
    {
        if (string.IsNullOrEmpty(roundDetail))
        {
            return string.Empty;
        }

        var history = JsonSerializer.Deserialize<AllBetGameHistory>(roundDetail)!;

        return history.GameType switch
        {
            101 => GetBaccaratGameHistory(history),
            102 => GetBaccaratGameHistory(history),
            103 => GetBaccaratGameHistory(history),
            104 => GetBaccaratGameHistory(history),
            201 => GetSicboGameHistory(history),
            301 => GetDragonTigerGameHistory(history),
            401 => GetRouletteGameHistory(history),
            501 => GetPokDengGameHistory(history),
            801 => GetBullBullGameHistory(history),
            901 => GetWinThreeCardsGameHistory(history),
            _ => GetBaccaratGameHistory(history)
        };
    }

    private string GetNumber(string value)
    {
        if (!int.TryParse(value, out var num))
        {
            return string.Empty;
        }

        return num switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => num.ToString()
        };
    }

    private string GetPokDengGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);

        var results = new Dictionary<string, string>();

        for (var i = 0; i < values.Length; i++)
        {
            var key = i == 0 ? "Banker" : $"Player{i}";
            var details = values[i].Split(',');
            var cards = GetCards(details);

            results[key] = cards;
        }

        return JsonSerializer.Serialize(results);
    }

    private string[] GetResultValues(string result)
    {
        var matches = Regex.Matches(result, "{.+?}");
        return matches.Select(c =>
                c.Value
                    .Replace("{", string.Empty)
                    .Replace("}", string.Empty))
            .ToArray();
    }

    private string GetRouletteGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);

        return JsonSerializer.Serialize(new { Result = values[0] });
    }

    private string GetSicboGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);
        var results = values[0].SplitByComma()
            .Select((v, i) => new
            {
                Value = v,
                DiceNumber = i + 1
            })
            .ToDictionary(v => $"Dice{v.DiceNumber}", v => v.Value);

        return JsonSerializer.Serialize(results);
    }

    /// <summary>
    ///     炸金花
    /// </summary>
    private string GetWinThreeCardsGameHistory(AllBetGameHistory history)
    {
        var values = GetResultValues(history.Result);

        var results = new Dictionary<string, string>();

        for (var i = 0; i < values.Length; i++)
        {
            var key = i == 0 ? "Dragon" : "Phoenix";
            var cards = GetCards(new[]
            {
                values[i]
            });

            results[key] = cards;
        }

        return JsonSerializer.Serialize(results);
    }
}
