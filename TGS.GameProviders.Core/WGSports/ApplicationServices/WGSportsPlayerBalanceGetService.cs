using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsPlayerBalanceGetService : IGetService<WGSportsPlayerBalanceQuery, decimal?>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsPlayerBalanceGetService> _logger;
        private readonly IWGSportsGamePlayer _playerCreate;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGSportsService _service;

        public WGSportsPlayerBalanceGetService(
            IWGSportsApiClient client,
            ILogger<WGSportsPlayerBalanceGetService> logger,
            IPlayerRepository playerRepository,
            IWGSportsGamePlayer playerCreate,
            IWGSportsService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<decimal?> GetAsync(WGSportsPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            (bool IsPlayer, decimal? balance) = await _client.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                playerModel.PlayerCode).ConfigureAwait(false);

            if (!IsPlayer)
            {
                if (await _playerCreate.CreatePlayerAsync(configuration, playerModel.PlayerCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false))
                {
                    (IsPlayer, balance) = await _client.GetBalanceAsync(
                        configuration.ApiUrl,
                        configuration.PartnerCode,
                        configuration.PartnerKey,
                        configuration.IV,
                        playerModel.PlayerCode).ConfigureAwait(false);
                }
            }

            return IsPlayer && balance.HasValue ? balance.Value : null;
        }
    }
}
