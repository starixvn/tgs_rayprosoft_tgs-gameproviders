using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.TCG;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class TCGGameRecordController : BaseController<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult>
    {
        /// <summary>
        /// Games the record asynchronous.
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
        public override ValueTask<TCGGameRecordHandlerResult> GameRecordAsync(
            [FromServices] IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult> service,
            TCGGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
