using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.TFGaming.ApplicationServices
{
    public class TFKickPlayerCommandService : ICommandService<TFKickPlayerCommand>
    {
        private readonly ITFApiClient _client;
        private readonly IPlayerService _playerService;
        private readonly ITFGamingService _service;

        public TFKickPlayerCommandService(
            ITFApiClient client,
            ITFGamingService service,
            IPlayerService playerService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask ExecuteAsync(TFKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerInfo = await _playerService
                .GetCacheAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (playerInfo == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(playerInfo.BrandCode, playerInfo.DefaultWalletCurrencyCode).ConfigureAwait(false);

            await _client
                .KickAsync(brandMerchant.ApiUrl, brandMerchant.PrivateToken, brandMerchant.PartnerId, command.PlayerCode)
                .ConfigureAwait(false);
        }
    }
}
