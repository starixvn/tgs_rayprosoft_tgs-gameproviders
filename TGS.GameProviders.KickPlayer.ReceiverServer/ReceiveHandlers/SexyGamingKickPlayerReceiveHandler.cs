using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.SexyGaming;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.ReceiveHandlers
{
    internal class SexyGamingKickPlayerReceiveHandler : IMessageHandler
    {
        private readonly ILogger<SexyGamingKickPlayerReceiveHandler> _logger;
        private readonly ICommandService<SexyGamingPlayerKickCommand> _commandService;

        public SexyGamingKickPlayerReceiveHandler(
            ILogger<SexyGamingKickPlayerReceiveHandler> logger,
            ICommandService<SexyGamingPlayerKickCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var request = JsonSerializer.Deserialize<SexyGamingPlayerKickCommand>(json);
            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(KYKickPlayerReceiveHandler)),
                request,
                null,
                (state, ex) => $"[{nameof(request.PlayerCode)}]{request.PlayerCode}");
            await _commandService.ExecuteAsync(request).ConfigureAwait(false);
        }
    }
}
