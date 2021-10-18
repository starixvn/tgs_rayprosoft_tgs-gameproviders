using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.TCG.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class TCGController : KickPlayerController<TCGKickPlayerCommand>
    {
        private readonly ILogger<TCGController> _logger;

        public TCGController(ILogger<TCGController> logger)
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
            [FromServices] ICommandService<TCGKickPlayerCommand> commandService,
            TCGKickPlayerCommand command)
            => commandService.ExecuteAsync(command);
    }
}
