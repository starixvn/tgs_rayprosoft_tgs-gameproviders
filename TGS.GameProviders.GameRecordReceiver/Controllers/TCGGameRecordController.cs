using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.TCG;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class TCGGameRecordController : BaseController<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult>
    {
        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Post 
        ///     Parameter
        ///     {
        ///         "subGP": "TCG_LOTTO",
        ///         "merchantCode": "pegacnys",
        ///         "isDesignation": true,
        ///         "lastGameRecorDirectory": "20210923",
        ///         "lastGameRecorFile": "202109231150_0001.json"
        ///     }
        /// </remarks>
        public override ValueTask<TCGGameRecordHandlerResult> KickPlayerAsync(
            [FromServices] IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult> service,
            TCGGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
