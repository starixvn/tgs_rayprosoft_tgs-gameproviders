using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.WGHorse;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class WGHorseGameRecordController : BaseController<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult>
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
        ///         "startTime": "2021-08-24T00:00:00.3527908+00:00",
        ///         "endTime": "2021-08-24T23:59:59.3527908+00:00"
        ///     }
        /// </remarks>
        public override ValueTask<WGHorseGameRecordHandlerResult> GameRecordAsync(
            [FromServices] IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult> service,
            WGHorseGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
