using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.VR;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.Controllers
{
    public class VRKickPlayerController : BaseController<VRPlayerKickCommand>
    {
        private readonly ILogger<VRKickPlayerController> _logger;

        public VRKickPlayerController(ILogger<VRKickPlayerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        /// Post
        ///     {
        ///      "playerCode": "4ucmqplxb029"
        ///     }
        /// </remarks>
        public override ValueTask KickPlayerAsync([FromServices] ICommandService<VRPlayerKickCommand> commandService, VRPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
