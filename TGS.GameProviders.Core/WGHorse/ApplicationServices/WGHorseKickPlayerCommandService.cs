using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorseKickPlayerCommandService : ICommandService<WGHorseKickPlayerCommand>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorseKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGHorseService _service;

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

        public async ValueTask ExecuteAsync(WGHorseKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode);

            if (gameProviderCurrencyCode == null)
            {
                return;
            }
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(configuration, new()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = command.PlayerCode,
                PlayerNickname = command.PlayerCode,
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
