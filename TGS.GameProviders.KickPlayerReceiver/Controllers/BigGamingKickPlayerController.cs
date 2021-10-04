using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayerReceiver.ApplicationServices.BigGaming;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.Controllers
{
    public class BigGamingKickPlayerController : BaseController<BigGamingPlayerKickCommand>
    {
        private readonly ILogger<BigGamingKickPlayerController> _logger;

        public BigGamingKickPlayerController(ILogger<BigGamingKickPlayerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override ValueTask KickPlayerAsync(
            [FromServices] ICommandService<BigGamingPlayerKickCommand> commandService,
            BigGamingPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
