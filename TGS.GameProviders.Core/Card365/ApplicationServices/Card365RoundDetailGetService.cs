using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Card365.ApplicationServices;

internal class Card365RoundDetailGetService : IGetService<Card365RoundDetailQuery, RoundDetailResult>
{
    private readonly ILogger<Card365RoundDetailGetService> _logger;
    private readonly IRoundRepository _roundRepository;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public Card365RoundDetailGetService(
        ILogger<Card365RoundDetailGetService> logger,
        IRoundRepository roundRepository,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));

        _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = false };
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        Card365RoundDetailQuery query,
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