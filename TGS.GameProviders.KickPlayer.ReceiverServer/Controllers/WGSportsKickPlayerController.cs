using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.WGSports;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.Controllers
{
    public class WGSportsKickPlayerController : BaseController<WGSportsPlayerKickCommand>
    {
        private readonly ILogger<WGSportsKickPlayerController> _logger;

        public WGSportsKickPlayerController(ILogger<WGSportsKickPlayerController> logger)
            => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        /// Post
        /// {
        /// "playerCode": "4ucmqplxb029"
        /// }
        /// </remarks>
        public override ValueTask KickPlayerAsync(
            [FromServices] ICommandService<WGSportsPlayerKickCommand> commandService, WGSportsPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}