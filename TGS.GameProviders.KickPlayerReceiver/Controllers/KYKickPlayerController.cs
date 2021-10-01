using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayerReceiver.ApplicationServices.KY;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.Controllers
{
    public class KYKickPlayerController : BaseController<KYPlayerKickCommand>
    {
        private readonly ILogger<KYKickPlayerController> _logger;

        public KYKickPlayerController(ILogger<KYKickPlayerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override ValueTask KickPlayerAsync(
            [FromServices] ICommandService<KYPlayerKickCommand> commandService,
            KYPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
