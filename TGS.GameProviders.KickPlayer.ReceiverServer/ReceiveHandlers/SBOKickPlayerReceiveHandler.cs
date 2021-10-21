using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.SBO;
using Tgs.MessageQueue;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.ReceiveHandlers
{
    public class SBOKickPlayerReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<SBOPlayerKickCommand> _commandService;
        private readonly ILogger<SBOKickPlayerReceiveHandler> _logger;

        public SBOKickPlayerReceiveHandler(
            ILogger<SBOKickPlayerReceiveHandler> logger,
            ICommandService<SBOPlayerKickCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var request = JsonSerializer.Deserialize<SBOPlayerKickCommand>(json);

            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(SBOKickPlayerReceiveHandler)),
                request,
                null,
                (state, ex) => $"[{nameof(request.PlayerCode)}]{request.PlayerCode}");

            return _commandService.ExecuteAsync(request, cancellationToken);
        }
    }
}