using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.PlayerBalance
{
    public class VRController : PlayerBalanceController<VRPlayerBalanceQuery, VRPlayerBalanceResult>
    {
        private readonly ILogger<VRController> _logger;

        public VRController(ILogger<VRController> logger)
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
        public override ValueTask<VRPlayerBalanceResult> PlayerBalanceAsync(
            [FromServices] IGetService<VRPlayerBalanceQuery, VRPlayerBalanceResult> getService,
            VRPlayerBalanceQuery query)
            => getService.GetAsync(query);
    }
}
