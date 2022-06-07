using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGaming.ApplicationServices
{
    public class TFKickPlayerCommandService : ICommandService<TFKickPlayerCommand>
    {
        private readonly ITFApiClient _client;
        private readonly IPlayerRepository _playerService;
        private readonly ITFGamingService _service;

        public TFKickPlayerCommandService(
            ITFApiClient client,
            ITFGamingService service,
            IPlayerRepository playerService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask ExecuteAsync(TFKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerService
                .GetAsync(command.PlayerCode)
                .ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

            await _client
                .KickAsync(brandMerchant.ApiUrl, brandMerchant.PrivateToken, brandMerchant.PartnerId, command.PlayerCode)
                .ConfigureAwait(false);
        }
    }
}
