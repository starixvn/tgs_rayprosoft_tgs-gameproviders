using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerFund
{
    public class KYController : PlayerFundController<KYPlayerFundInQuery, KYPlayerFundOutQuery, PlayerFundResult>
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
        /// Players the fund in asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY",
        ///             "amount": 1
        ///         }
        /// </remarks>
        public override async ValueTask<PlayerFundResponse> PlayerFundInAsync(
            [FromServices] IGetService<KYPlayerFundInQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<KYPlayerFundInQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
        }

        /// <summary>
        /// Players the fund out asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY",
        ///             "amount": 1
        ///         }
        /// </remarks>
        public override async ValueTask<PlayerFundResponse> PlayerFundOutAsync(
            [FromServices] IGetService<KYPlayerFundOutQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<KYPlayerFundOutQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
        }
    }
}
