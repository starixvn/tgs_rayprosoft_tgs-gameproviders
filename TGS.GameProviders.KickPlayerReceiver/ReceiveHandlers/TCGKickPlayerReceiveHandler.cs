using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.TCG;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.ReceiveHandlers
{
    internal class TCGKickPlayerReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<TCGPlayerKickCommand> _commandService;
        private readonly ILogger<BigGamingKickPlayerReceiveHandler> _logger;

        public TCGKickPlayerReceiveHandler(
            ILogger<BigGamingKickPlayerReceiveHandler> logger,
            ICommandService<TCGPlayerKickCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var request = JsonSerializer.Deserialize<TCGPlayerKickCommand>(json);
            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(KYKickPlayerReceiveHandler)),
                request,
                null,
                (state, ex) => $"[{nameof(request.PlayerCode)}]{request.PlayerCode}");
            await _commandService.ExecuteAsync(request).ConfigureAwait(false);
        }
    }
}
