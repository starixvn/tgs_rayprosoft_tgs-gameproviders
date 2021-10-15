using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.VR;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.ReceiveHandlers
{
    internal class VRKickPlayerReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<VRPlayerKickCommand> _commandService;
        private readonly ILogger<VRKickPlayerReceiveHandler> _logger;

        public VRKickPlayerReceiveHandler(
            ILogger<VRKickPlayerReceiveHandler> logger,
            ICommandService<VRPlayerKickCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var request = JsonSerializer.Deserialize<VRPlayerKickCommand>(json);
            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(VRKickPlayerReceiveHandler)),
                request,
                null,
                (state, ex) => $"[{nameof(request.PlayerCode)}]{request.PlayerCode}");
            await _commandService.ExecuteAsync(request).ConfigureAwait(false);
        }
    }
}
