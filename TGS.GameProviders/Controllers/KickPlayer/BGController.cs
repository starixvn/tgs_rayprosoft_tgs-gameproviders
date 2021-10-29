using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class BGController : KickPlayerController<BigGamingKickPlayerCommand>
    {
        private readonly ILogger<BGController> _logger;
        private readonly IMapper _mapper;

        public BGController(
            ILogger<BGController> logger,
            IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="request">The request.</param>
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
                 KickPlayerRequest request)
            => commandService.ExecuteAsync(_mapper.Map<BigGamingKickPlayerCommand>(request));
    }
}
