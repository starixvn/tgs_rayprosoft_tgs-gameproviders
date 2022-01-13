using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.TFGaming.ApplicationServices
{
    public class TFKickPlayerCommandService : ICommandService<TFKickPlayerCommand>
    {
        private readonly ITFApiClient _client;
        private readonly ITFGamingService _service;
        private readonly IPlayerRepository _playerRepository;

        public TFKickPlayerCommandService(
            ITFApiClient client,
            ITFGamingService service,
            IPlayerRepository playerRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(TFKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository
                .GetAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

            await _client
                .KickAsync(brandMerchant.ApiUrl, brandMerchant.PrivateToken, brandMerchant.PartnerId, command.PlayerCode)
                .ConfigureAwait(false);
        }
    }
}