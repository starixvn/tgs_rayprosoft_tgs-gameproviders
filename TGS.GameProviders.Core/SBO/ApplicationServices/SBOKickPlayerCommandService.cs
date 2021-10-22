using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.SBO;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.SBO.ApplicationServices
{
    internal class SBOKickPlayerCommandService : ICommandService<SBOKickPlayerCommand>
    {
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOKickPlayerCommandService> _logger;

        public SBOKickPlayerCommandService(
            ISBOApiClient client,
            ILogger<SBOKickPlayerCommandService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(SBOKickPlayerCommand command, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}