using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYKickPlayerCommandService : ICommandService<LYKickPlayerCommand>
    {
        private readonly ILYApiClient _client;
        private readonly ILogger<LYKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILYService _service;

        public LYKickPlayerCommandService(
            ILogger<LYKickPlayerCommandService> logger,
            ILYApiClient client,
            ILYService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(LYKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);
            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);
            await _client.KickAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, new()
            {
                PlayerCode = command.PlayerCode
            }).ConfigureAwait(false);
        }
    }
}
