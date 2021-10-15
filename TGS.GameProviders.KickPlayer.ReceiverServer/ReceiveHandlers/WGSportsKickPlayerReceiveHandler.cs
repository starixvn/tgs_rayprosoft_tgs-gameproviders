using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.WGSports;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.ReceiveHandlers
{
    internal class WGSportsKickPlayerReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<WGSportsPlayerKickCommand> _commandService;
        private readonly ILogger<WGSportsKickPlayerReceiveHandler> _logger;

        public WGSportsKickPlayerReceiveHandler(
            ILogger<WGSportsKickPlayerReceiveHandler> logger,
            ICommandService<WGSportsPlayerKickCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var request = JsonSerializer.Deserialize<WGSportsPlayerKickCommand>(json);
            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(WGSportsKickPlayerReceiveHandler)),
                request,
                null,
                (state, ex) => $"[{nameof(request.PlayerCode)}]{request.PlayerCode}");
            await _commandService.ExecuteAsync(request).ConfigureAwait(false);
        }
    }
}
