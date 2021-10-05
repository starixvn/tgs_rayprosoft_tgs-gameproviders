using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.WGSports;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class WGSportsGameRecordController : BaseController<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult>
    {
        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        /// Post
        ///     {
        ///      "startTime": "2021-09-24T02:00:00+00:00",
        ///      "endTime": "2021-09-24T04:00:00+00:00"
        ///     }
        /// </remarks>
        public override ValueTask<WGSportsGameRecordHandlerResult> KickPlayerAsync([FromServices] IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> service, WGSportsGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
