using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.GameUrl
{
    public class KYController : GameUrlController<KYGameUrlQuery, KYGameUrlResult>
    {
        private readonly ILogger<KYController> _logger;

        public KYController(ILogger<KYController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Games the URL asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        /// Post
        ///     {
        ///         "gameType": 9,
        ///         "gameId": 1000031,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY",
        ///         "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": ""
        ///     }
        /// </remarks>
        public override ValueTask<KYGameUrlResult> GameUrlAsync(
            [FromServices] IGetService<KYGameUrlQuery, KYGameUrlResult> getService,
            KYGameUrlQuery query)
            => getService.GetAsync(query);
    }
}
