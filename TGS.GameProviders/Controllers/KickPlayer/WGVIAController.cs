using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class WGVIAController : KickPlayerController<WGHorseKickPlayerCommand>
    {
        private readonly ILogger<WGVIAController> _logger;

        public WGVIAController(ILogger<WGVIAController> logger)
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
        public override ValueTask KickPlayerAsync([FromServices] ICommandService<WGHorseKickPlayerCommand> commandService, WGHorseKickPlayerCommand command)
            => commandService.ExecuteAsync(command);
    }
}
