using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.SBO.ApplicationServices
{
    internal class SBOKickPlayerCommandService : ICommandService<SBOKickPlayerCommand>
    {
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOKickPlayerCommandService> _logger;
        private readonly ISBOService _service;

        public SBOKickPlayerCommandService(
            ISBOApiClient client,
            ILogger<SBOKickPlayerCommandService> logger,
            ISBOService service)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask ExecuteAsync(SBOKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            await _client.LogoutAsync(configuration.ApiUrl, new()
            {
                CompanyKey = configuration.CompanyKey,
                PlayerCode = command.PlayerCode
            }).ConfigureAwait(false);
        }
    }
}