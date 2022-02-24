using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorsePlayerBalanceGetService : IGetService<WGHorsePlayerBalanceQuery, decimal?>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorsePlayerBalanceGetService> _logger;
        private readonly IWGHorseGamePlayer _playerCreate;
        private readonly IPlayerService _playerService;
        private readonly IWGHorseService _service;

        public WGHorsePlayerBalanceGetService(
            IWGHorseApiClient client,
            ILogger<WGHorsePlayerBalanceGetService> logger,
            IWGHorseGamePlayer playerCreate,
            IPlayerService playerService,
            IWGHorseService service
            )
        {
            if (logger is null) throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<decimal?> GetAsync(WGHorsePlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerInfo == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            (bool IsPlayer, decimal? Balance) = await _client.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                playerInfo.PlayerCode).ConfigureAwait(false);

            if (!IsPlayer)
            {
                if (await _playerCreate.CreatePlayerAsync(configuration, playerInfo.PlayerCode, playerInfo.DefaultWalletCurrencyCode).ConfigureAwait(false))
                {
                    (IsPlayer, Balance) = await _client.GetBalanceAsync(
                        configuration.ApiUrl,
                        configuration.PartnerCode,
                        configuration.PartnerKey,
                        configuration.IV,
                        playerInfo.PlayerCode).ConfigureAwait(false);
                }
            }

            return IsPlayer && Balance.HasValue ? Balance.Value : null;
        }
    }
}
