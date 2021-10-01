using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayerReceiver.ApplicationServices.WGHorse;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.Controllers
{
    public class WGHorseKickPlayerController : BaseController<WGHorsePlayerKickCommand>
    {
        private readonly ILogger<WGHorseKickPlayerController> _logger;

        public WGHorseKickPlayerController(ILogger<WGHorseKickPlayerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override ValueTask KickPlayerAsync([FromServices] ICommandService<WGHorsePlayerKickCommand> commandService, WGHorsePlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
