using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerBalance
{
    public class KYController : PlayerBalanceController<KYPlayerBalanceQuery, KYPlayerBalanceResult>
    {
        private readonly ILogger<KYController> _logger;

        public KYController(ILogger<KYController> logger)
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
        public override ValueTask<KYPlayerBalanceResult> PlayerBalanceAsync(
            [FromServices] IGetService<KYPlayerBalanceQuery, KYPlayerBalanceResult> getService,
            KYPlayerBalanceQuery query)
            => getService.GetAsync(query);
    }
}
