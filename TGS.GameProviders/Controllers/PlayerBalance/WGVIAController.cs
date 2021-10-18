using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerBalance
{
    public class WGVIAController : PlayerBalanceController<WGHorsePlayerBalanceQuery, WGHorsePlayerBalanceResult>
    {
        private readonly ILogger<WGVIAController> _logger;

        public WGVIAController(ILogger<WGVIAController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="query">The query.</param>
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
        public override ValueTask<WGHorsePlayerBalanceResult> PlayerBalanceAsync(
            [FromServices] IGetService<WGHorsePlayerBalanceQuery, WGHorsePlayerBalanceResult> getService,
            WGHorsePlayerBalanceQuery query)
            => getService.GetAsync(query);
    }
}
