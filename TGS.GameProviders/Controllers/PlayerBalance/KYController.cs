using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerBalance
{
    public class KYController : PlayerBalanceController<KYPlayerBalanceQuery, PlayerBalanceResult>
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
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
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
        public override async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
            [FromServices] IGetService<KYPlayerBalanceQuery, PlayerBalanceResult> getService,
            PlayerBalanceRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<KYPlayerBalanceQuery>(request)).ConfigureAwait(false);
            return _mapper.Map<PlayerBalanceResponse>(result);
        }
    }
}
