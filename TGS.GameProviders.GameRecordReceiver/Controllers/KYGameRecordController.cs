using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.Ky;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class KYGameRecordController : BaseController<KYGameRecordHandlerQuery, KYGameRecordHandlerResult>
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
        ///         "brandCode":"Default",
        ///         "currencyCode":"CNY",
        ///         "startTime":"2021-08-12T05:50:00+00:00",
        ///         "endTime":"2021-08-12T06:00:00+00:00"
        ///     }
        /// </remarks>
        public override ValueTask<KYGameRecordHandlerResult> KickPlayerAsync(
            [FromServices] IGetService<KYGameRecordHandlerQuery, KYGameRecordHandlerResult> service,
            KYGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
