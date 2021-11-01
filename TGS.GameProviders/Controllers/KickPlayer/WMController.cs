using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WMCasino.ApplicationServices;
using TGS.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class WMController : KickPlayerController<WMCasinoKickPlayerCommand>
    {
        private readonly ILogger<WMController> _logger;
        private readonly IMapper _mapper;

        public WMController(
            ILogger<WMController> logger,
            IMapper mapper)
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
            ICommandService<WMCasinoKickPlayerCommand> commandService, KickPlayerRequest request)
            => commandService.ExecuteAsync(_mapper.Map<WMCasinoKickPlayerCommand>(request));
    }
}