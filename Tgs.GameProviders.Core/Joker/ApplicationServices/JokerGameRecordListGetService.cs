using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerGameRecordListGetService : IGetService<JokerGameRecordListQuery, JokerGameRecordResult>
{
    private readonly IJokerAgentMapper _agentMapper;
    private readonly IGameInterfaceJokerClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IJokerService _service;

    public JokerGameRecordListGetService(
        IGameInterfaceJokerClient client,
        IGameProviderRepository gameProvider,
        IJokerService service,
        IJokerAgentMapper agentMapper)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
    }

    public async ValueTask<JokerGameRecordResult> GetAsync(
        JokerGameRecordListQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgentById(configuration!.BrandMerchants, query.BrandCode, query.AgentId)
            ?? throw new Exception($"The agent does not exist of brand '{query.BrandCode}' and agent id '{query.AgentId}'");

        var reply = await _client
            .GameRecordAsync(
                apiUrl: configuration.ApiUrl,
                appId: configuration.AppId,
                secretKey: configuration.SecretKey,
                startTime: query.StartTime,
                endTime: query.EndTime,
                nextId: query.NextId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = GameProviderType.Joker;
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: gameProviderCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameRecords = reply.Items.Select(data =>
        {
            return new GameRecordListModel
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.PlayerCode.ToLower(),
                Status = GameRoundStatus.Closed,
                ClientStatus = GetClientStatus(data.WinLoss.ToDecimal().GetValueOrDefault(0)),
                ExternalRoundId = data.ExternalRoundId,
                ExternalGameId = data.ExternalGameId,
                ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset(),
                ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset(),
                BetAmount = data.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount = data.Bet.ToDecimal().GetValueOrDefault(0),
                SettleAmount = data.Settle.ToDecimal().GetValueOrDefault(0),
                PlayerWinLoss = data.WinLoss.ToDecimal().GetValueOrDefault(0),
                Commission = 0,
                Turnover = data.Bet.ToDecimal().GetValueOrDefault(0),
                TieBetAdjustment = null,
                AdditionalData = data.AdditionalData
            };
        });
        return new JokerGameRecordResult
        {
            Success = true,
            NextId = reply.NextId,
            Datas = gameRecords.ToArray()
        };
    }

    private static GameRoundClientStatus GetClientStatus(decimal playerWinLoss)
        => playerWinLoss switch
        {
            > 0 => GameRoundClientStatus.Win,
            _ => GameRoundClientStatus.Loss
        };
}
