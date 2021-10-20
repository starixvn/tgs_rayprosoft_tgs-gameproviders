using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class BGController : KickPlayerController<BigGamingKickPlayerCommand>
    {
        private readonly ILogger<BGController> _logger;

        public BGController(ILogger<BGController> logger)
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
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY"
        ///     }
        /// </remarks>
        public override ValueTask KickPlayerAsync(
                [FromServices] ICommandService<BigGamingKickPlayerCommand> commandService,
                BigGamingKickPlayerCommand command)
                => commandService.ExecuteAsync(command);
    }
}
