using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.WGHorse.Models;

namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorsePlayerBalanceGetService : IGetService<WGHorsePlayerBalanceQuery, decimal?>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorsePlayerBalanceGetService> _logger;
        private readonly IWGHorseGamePlayer _playerCreate;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGHorseService _service;

        public WGHorsePlayerBalanceGetService(
            IWGHorseApiClient client,
            ILogger<WGHorsePlayerBalanceGetService> logger,
            IWGHorseGamePlayer playerCreate,
            IPlayerRepository playerRepository,
            IWGHorseService service
            )
        {
            if (logger is null) throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<decimal?> GetAsync(WGHorsePlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var source = new WGHorseGetBalanceRequest()
            {
                PlayerCode = playerModel.PlayerCode
            };
            var response = await _client.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                source).ConfigureAwait(false);

            if (response.ReturnCode == 602)
            {
                if (await _playerCreate.CreatePlayerAsync(configuration, playerModel.PlayerCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false))
                {
                    response = await _client.GetBalanceAsync(
                        configuration.ApiUrl,
                        configuration.PartnerCode,
                        configuration.PartnerKey,
                        configuration.IV,
                        source).ConfigureAwait(false);
                }
            }

            return response.ReturnCode == 1 ? decimal.Parse(response.Balance) : null;
        }
    }
}
