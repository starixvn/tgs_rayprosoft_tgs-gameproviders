using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices
{
    internal class DreamGamingTransferRoundDetailGetService : IGetService<DreamGamingTransferRoundDetailQuery, RoundDetailResult>
    {
        private readonly IBrandRepository _brand;
        private readonly IGameInterfaceDreamGamingClient _client;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IPlayerRepository _player;
        private readonly IRoundRepository _roundService;

        public DreamGamingTransferRoundDetailGetService(
            IRoundRepository roundService,
            IGameInterfaceDreamGamingClient client,
            IBrandRepository brand,
            IGameProviderRepository gameProvider,
            IPlayerRepository player)
        {
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public async ValueTask<RoundDetailResult> GetAsync(DreamGamingTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundInfo = await _roundService
                .GetAsync(
                roundId: query.RoundId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"This game round number could not be found.{query.RoundId}");

            var gameProviderId = await _gameProvider
                .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.DreamGamingTransfer,
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
                .GetDreamGamingTransferConfigurationAsync(
                gameProviderConfigurationId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProvider
                .GetGameProviderCultureCodeAsync(
                gameProviderId,
                query.Language,
                cancellationToken)
                .ConfigureAwait(false) ?? "cn";

            var reply = await _client
                .GameHistoryDetailAsync(
                    configuration.VideoGameApiUrl,
                    configuration.AgentName,
                    gameProviderCultureCode,
                    roundInfo.ExternalRoundId,
                    cancellationToken)
                .ConfigureAwait(false);

            return new RoundDetailResult()
            {
                Data = new RoundDetailDataBase[]
                    {
                    new RoundDetailDataUrl { Url = reply.GameResult }
                    }
            };
        }
    }
}
