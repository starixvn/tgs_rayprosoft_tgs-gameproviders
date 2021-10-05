using System;
using System.Threading.Tasks;
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
        public override ValueTask KickPlayerAsync(
            [FromServices] ICommandService<BigGamingPlayerKickCommand> commandService,
            BigGamingPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}
