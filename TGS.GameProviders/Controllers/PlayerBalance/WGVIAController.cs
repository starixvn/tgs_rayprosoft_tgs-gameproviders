using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers.PlayerBalance
{
    public class WGVIAController : PlayerBalanceController<WGHorsePlayerBalanceQuery>
    {
        private readonly ILogger<WGVIAController> _logger;
        private readonly IMapper _mapper;

        public WGVIAController(
            ILogger<WGVIAController> logger,
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
        public override async ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(
            [FromServices] IGetService<WGHorsePlayerBalanceQuery, PlayerBalanceResult> getService,
            PlayerBalanceRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WGHorsePlayerBalanceQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerBalanceResponse>(result);
        }
    }
}
