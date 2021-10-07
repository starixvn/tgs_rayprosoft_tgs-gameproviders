using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.WGHorse;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.ReceiveHandlers
{
    internal class WGHorseKickPlayerReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<WGHorsePlayerKickCommand> _commandService;
        private readonly ILogger<WGHorseKickPlayerReceiveHandler> _logger;

        public WGHorseKickPlayerReceiveHandler(
            ILogger<WGHorseKickPlayerReceiveHandler> logger,
            ICommandService<WGHorsePlayerKickCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var request = JsonSerializer.Deserialize<WGHorsePlayerKickCommand>(json);
            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(WGHorsePlayerKickCommand)),
                request,
                null,
                (state, ex) => $"[{nameof(request.PlayerCode)}]{request.PlayerCode}");
            await _commandService.ExecuteAsync(request).ConfigureAwait(false);
        }
    }
}
