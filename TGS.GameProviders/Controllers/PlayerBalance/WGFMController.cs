using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerBalance
{
    public class WGFMController : PlayerBalanceController<WGSportsPlayerBalanceQuery, WGSportsPlayerBalanceResult>
    {
        private readonly ILogger<WGFMController> _logger;

        public WGFMController(ILogger<WGFMController> logger)
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
        public override ValueTask<WGSportsPlayerBalanceResult> PlayerBalanceAsync(
            [FromServices] IGetService<WGSportsPlayerBalanceQuery, WGSportsPlayerBalanceResult> getService,
            WGSportsPlayerBalanceQuery query)
            => getService.GetAsync(query);
    }
}
