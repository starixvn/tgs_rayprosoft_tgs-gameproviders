using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorsePlayerBalanceGetService : IGetService<WGHorsePlayerBalanceQuery, decimal?>
    {
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseGamePlayer _playerCreate;
        private readonly IPlayerRepository _playerService;
        private readonly IWGHorseService _service;

        public WGHorsePlayerBalanceGetService(
            IWGHorseApiClient client,
            IWGHorseGamePlayer playerCreate,
            IPlayerRepository playerService,
            IWGHorseService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<decimal?> GetAsync(WGHorsePlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerService.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            (bool IsPlayer, decimal? Balance) = await _client.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                playerModel.PlayerCode).ConfigureAwait(false);

            if (!IsPlayer)
            {
                if (await _playerCreate.CreatePlayerAsync(configuration, playerModel.PlayerCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false))
                {
                    (IsPlayer, Balance) = await _client.GetBalanceAsync(
                        configuration.ApiUrl,
                        configuration.PartnerCode,
                        configuration.PartnerKey,
                        configuration.IV,
                        playerModel.PlayerCode).ConfigureAwait(false);
                }
            }

            return IsPlayer && Balance.HasValue ? Balance.Value : null;
        }
    }
}
