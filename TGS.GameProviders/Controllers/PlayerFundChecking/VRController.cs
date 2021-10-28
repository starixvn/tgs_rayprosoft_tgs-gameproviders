using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerFundChecking
{
    [ApiController]
    [Route("api/PlayerFundChecking/[controller]")]
    public class VRController : ControllerBase
    {
        private readonly ILogger<VRController> _logger;
        private readonly IMapper _mapper;

        public VRController(
            ILogger<VRController> logger,
            IMapper mapper
           )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Players the fund checking asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY",
        ///             "orderId": "D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
        ///             "startTime": "2021-10-27T00:00:00.000Z",
        ///             "endTime": "2021-10-28T00:00:00.000Z"
        ///         }
        /// </remarks>
        [HttpPost]
        public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
                [FromServices] IGetService<VRPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
                VRPlayerFundCheckingRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<VRPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundCheckingResponse>(result);
        }
    }
}
