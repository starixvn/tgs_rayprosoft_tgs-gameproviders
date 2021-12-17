using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.Jili.ApplicationServices
{
    internal class JiliKickPlayerCommandService : ICommandService<JiliKickPlayerCommand>
    {
        private readonly ILogger<JiliKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IJiliService _service;
        private readonly IJiliMerchantMapper _merchantMapper;
        private readonly IJiliApiClient _client;

        public JiliKickPlayerCommandService(
            ILogger<JiliKickPlayerCommandService> logger,
            IPlayerRepository playerRepository,
            IJiliService service,
            IJiliMerchantMapper merchantMapper,
            IJiliApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask ExecuteAsync(
            JiliKickPlayerCommand command,
            CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var merchant = _merchantMapper.GetMerchant(configuration.Merchants, playerModel.BrandCode);
            var jiliPlayerId = $"{playerModel.BrandCode}_{playerModel.PlayerCode}";

            await _client.KickMemberAsync(
                    configuration.ApiUrl,
                    jiliPlayerId,
                    merchant.AgentId,
                    merchant.AgentKey)
                .ConfigureAwait(false);
        }
    }
}