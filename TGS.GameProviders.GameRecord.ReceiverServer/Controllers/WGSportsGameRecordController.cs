using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.WGSports;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class WGSportsGameRecordController : BaseController<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult>
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
        ///         "startTime": "2021-10-26T05:13:00+00:00",
        ///         "endTime": "2021-10-26T07:45:13+00:00"
        ///     }
        /// </remarks>
        public override ValueTask<WGSportsGameRecordHandlerResult> GameRecordAsync(
            [FromServices] IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> service,
            WGSportsGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
