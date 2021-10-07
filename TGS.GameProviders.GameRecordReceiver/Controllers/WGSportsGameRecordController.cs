using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.WGSports;
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
        ///     Post
        ///     Parameter
        ///     {
        ///         "startTime": "2021-08-24T00:00:00.3527908+00:00",
        ///         "endTime": "2021-08-24T23:59:59.3527908+00:00"
        ///     }
        /// </remarks>
        public override ValueTask<WGSportsGameRecordHandlerResult> KickPlayerAsync(
            [FromServices] IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> service,
            WGSportsGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
