using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SM01.ApplicationServices
{
    internal class SM01PlatformLoginGetService : IGetService<SM01PlatformLoginQuery, SM01PlatformLoginInfo>
    {
        private readonly IBrandRepository _brand;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IPlayerRepository _player;

        public SM01PlatformLoginGetService(
            IBrandRepository brand,
            IGameProviderRepository gameProvider,
            IPlayerRepository player)
        {
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public async ValueTask<SM01PlatformLoginInfo> GetAsync(SM01PlatformLoginQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SM01).ConfigureAwait(false);
            var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configuration = await _gameProvider.GetSM01Configuration(gameProviderConfigurationId).ConfigureAwait(false);

            return new SM01PlatformLoginInfo(configuration.ApiUrl, configuration.MerchantId, configuration.MerchantKey);
        }
    }
}
