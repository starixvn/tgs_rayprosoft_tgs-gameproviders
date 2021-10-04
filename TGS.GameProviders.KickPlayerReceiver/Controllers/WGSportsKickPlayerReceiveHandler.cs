using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayerReceiver.ApplicationServices.WGSports;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.Controllers
{
    public class WGSportsKickPlayerReceiveHandler : BaseController<WGSportsPlayerKickCommand>
    {
        private readonly ILogger<WGSportsKickPlayerReceiveHandler> _logger;

        public WGSportsKickPlayerReceiveHandler(ILogger<WGSportsKickPlayerReceiveHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override ValueTask KickPlayerAsync([FromServices] ICommandService<WGSportsPlayerKickCommand> commandService, WGSportsPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
