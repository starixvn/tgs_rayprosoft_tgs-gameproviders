using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerGameRecordListGetService : IGetService<JokerGameRecordListQuery, JokerGameRecordResult>
{
    private readonly IJokerApiClient _client;
    private readonly IJokerService _service;
    private readonly IJokerAgentMapper _agentMapper;

    public JokerGameRecordListGetService(
        IJokerApiClient client,
        IJokerService service,
        IJokerAgentMapper agentMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
    }

    public async ValueTask<JokerGameRecordResult> GetAsync(
        JokerGameRecordListQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgentById(configuration!.BrandMerchants, query.BrandCode, query.AgentId);

        if (agent == null)
        {
            throw new Exception($"The agent does not exist of brand '{query.BrandCode}' and agent id '{query.AgentId}'");
        }

        var (success, response) = await _client
            .GetGameRecordAsync(
                configuration.ApiUrl,
                query.StartTime,
                query.EndTime,
                query.NextId,
                agent.AgentId,
                agent.SecretKey)
            .ConfigureAwait(false);

        if (!success)
        {
            return new JokerGameRecordResult
            {
                Success = false
            };
        }

        var gameProviderCode = GameProviderType.Joker;
        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

        var gameRecords = response.Data.Game.Select(data =>
        {
            var playerWinLoss = data.SettleAmount - data.BetAmount;

            return new GameRecordListModel
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.Username.ToLower(),
                Status = GameRoundStatus.Closed,
                ClientStatus = GetClientStatus(playerWinLoss),
                ExternalRoundId = data.TransactionId,
                ExternalGameId = data.GameExternalId,
                ExternalCreatedOn = data.Time,
                ExternalClosedOn = data.Time,
                BetAmount = data.BetAmount,
                ValidBetAmount = data.BetAmount,
                PlayerWinLoss = playerWinLoss,
                Commission = 0,
                Turnover = data.BetAmount,
                SettleAmount = data.SettleAmount,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(data)
            };
        });

        var jackpotRecords = response.Data.Jackpot.Select(data =>
        {
            var playerWinLoss = data.SettleAmount - data.BetAmount;

            return new GameRecordListModel
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.Username,
                Status = GameRoundStatus.Closed,
                ClientStatus = GetClientStatus(playerWinLoss),
                ExternalRoundId = data.TransactionId,
                ExternalGameId = data.GameExternalId,
                ExternalCreatedOn = data.Time,
                ExternalClosedOn = data.Time,
                BetAmount = data.BetAmount,
                ValidBetAmount = data.BetAmount,
                PlayerWinLoss = playerWinLoss,
                Commission = 0,
                Turnover = data.BetAmount,
                SettleAmount = data.SettleAmount,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(data)
            };
        });

        var competitionRecords = response.Data.Competition.Select(data =>
        {
            var playerWinLoss = data.SettleAmount - data.BetAmount;

            return new GameRecordListModel
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.Username,
                Status = GameRoundStatus.Closed,
                ClientStatus = GetClientStatus(playerWinLoss),
                ExternalRoundId = data.TransactionId,
                ExternalGameId = data.GameExternalId,
                ExternalCreatedOn = data.Time,
                ExternalClosedOn = data.Time,
                BetAmount = data.BetAmount,
                ValidBetAmount = data.BetAmount,
                PlayerWinLoss = playerWinLoss,
                Commission = 0,
                Turnover = data.BetAmount,
                SettleAmount = data.SettleAmount,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(data)
            };
        });

        var records = gameRecords.Concat(jackpotRecords).Concat(competitionRecords).ToArray();

        return new JokerGameRecordResult
        {
            Success = true,
            NextId = response.NextId,
            Datas = records
        };
    }

    private static GameRoundClientStatus GetClientStatus(decimal playerWinLoss)
        => playerWinLoss switch
        {
            > 0 => GameRoundClientStatus.Win,
            _ => GameRoundClientStatus.Loss
        };
}