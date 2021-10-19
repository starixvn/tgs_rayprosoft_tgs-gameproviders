using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.WGSports
{
    public class WGSportsKickPlayerCommandService : ICommandService<WGSportsPlayerKickCommand>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGSportsService _service;

        public WGSportsKickPlayerCommandService(
            ILogger<WGSportsKickPlayerCommandService> logger,
            IWGSportsApiClient client,
            IWGSportsService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(WGSportsPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            if (player == null)
            {
                return;
            }
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(player.DefaultWalletCurrencyCode);

            if (gameProviderCurrencyCode == null)
            {
                return;
            }
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                new()
                {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    PlayerCode = player.PlayerCode,
                    PlayerNickname = player.PlayerCode,
                    CurrencyCode = gameProviderCurrencyCode
                }).ConfigureAwait(false);

            if (prepareLoginResponse.ReturnCode is not 1)
            {
                return;
            }

            _ = await _client.GetLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                new()
                {
                    Token = prepareLoginResponse.Token,
                    Lang = "0"
                }).ConfigureAwait(false);
        }
    }
}
