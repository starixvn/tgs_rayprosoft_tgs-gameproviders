using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class KYController : KickPlayerController<KYKickPlayerCommand>
    {
        private readonly ILogger<KYController> _logger;
        private readonly IMapper _mapper;

        public KYController(
            ILogger<KYController> logger,
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
            [FromServices] ICommandService<KYKickPlayerCommand> commandService,
            KickPlayerRequest request)
             => commandService.ExecuteAsync(_mapper.Map<KYKickPlayerCommand>(request));
    }
}
