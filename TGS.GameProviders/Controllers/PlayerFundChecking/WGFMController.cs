using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerFundChecking
{
    public class WGFMController : PlayerFundCheckingController<WGSportsPlayerFundCheckingQuery>
    {
        private readonly ILogger<WGFMController> _logger;
        private readonly IMapper _mapper;

        public WGFMController(
            ILogger<WGFMController> logger,
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
        ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86"
        ///         }
        /// </remarks>
        public override async ValueTask<bool> PlayerFundCheckingAsync(
            [FromServices] IGetService<WGSportsPlayerFundCheckingQuery, bool> getService,
            PlayerFundCheckingRequest request)
            => await getService.GetAsync(_mapper.Map<WGSportsPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
    }
}
