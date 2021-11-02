using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.LY;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class LYGameRecordController : BaseController<LYGameRecordHandlerQuery, LYGameRecordHandlerResult>
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
        ///         "brandCode":"Default",
        ///         "currencyCode":"CNY",
        ///         "startTime":"2021-10-28T13:00:00+08:00",
        ///         "endTime":"2021-10-28T13:40:00+08:00"
        ///     }
        /// </remarks>
        public override ValueTask<LYGameRecordHandlerResult> GameRecordAsync(
            [FromServices] IGetService<LYGameRecordHandlerQuery, LYGameRecordHandlerResult> service,
            LYGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
