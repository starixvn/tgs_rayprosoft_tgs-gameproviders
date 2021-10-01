using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayerReceiver.ApplicationServices.VR;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.Controllers
{
    public class VRKickPlayerController : BaseController<VRPlayerKickCommand>
    {
        private readonly ILogger<VRKickPlayerController> _logger;

        public VRKickPlayerController(ILogger<VRKickPlayerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override ValueTask KickPlayerAsync([FromServices] ICommandService<VRPlayerKickCommand> commandService, VRPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
