using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.WGHorse;
using TGS.GameProviders.ApiClients;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.WGHorse
{
    public class WGHorseKickPlayerCommandService : ICommandService<WGHorsePlayerKickCommand>
    {
        private readonly ILogger<WGHorseKickPlayerCommandService> _logger;
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseService _service;
        private readonly IPlayerRepository _playerRepository;

        public WGHorseKickPlayerCommandService(
            ILogger<WGHorseKickPlayerCommandService> logger,
            IWGHorseApiClient client,
            IWGHorseService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(WGHorsePlayerKickCommand command, CancellationToken cancellationToken = default)
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
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(configuration, new()
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

            _ = await _client.GetLoginAsync(configuration, new()
            {
                Token = prepareLoginResponse.Token,
                Lang = "0"
            }).ConfigureAwait(false);
        }
    }
}
