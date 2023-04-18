using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

internal class JiliTransferRoundDetailGetService : IGetService<JiliTransferRoundDetailQuery, RoundDetailResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceJiliClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly IRoundService _roundService;

    public JiliTransferRoundDetailGetService(
        IRoundService roundService,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceJiliClient client)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(JiliTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
                query.RoundId)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game round number could not be found.{query.RoundId}");

        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.JiliTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: roundInfo.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetJiliTransferConfiguration(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: gameProviderId,
                cultureCode: query.Language,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en-US";

        var result = await _client.GetGameDetailUrlAsync(
                    apiUrl: configuration.ApiUrl,
                    agentId: configuration.AgentId,
                    agentKey: configuration.AgentKey,
                    wagersId: roundInfo.ExternalRoundId,
                    language: cultureCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = result
                }
            }
        };
    }
}
