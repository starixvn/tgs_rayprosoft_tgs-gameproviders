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
        private readonly IRoundService _roundService;

        public DreamGamingTransferRoundDetailGetService(
            IRoundService roundService,
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
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);

            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.DreamGamingTransfer).ConfigureAwait(false);
            var player = await _player.GetAsync(roundInfo.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configuration = await _gameProvider.GetDreamGamingTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, query.Language).ConfigureAwait(false);

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
