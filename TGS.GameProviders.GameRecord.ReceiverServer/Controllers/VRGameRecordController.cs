using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.VR;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class VRGameRecordController : BaseController<VRGameRecordHandlerQuery, VRGameRecordHandlerResult>
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
        ///         "startTime":"2021-10-28T03:50:00",
        ///         "endTime":"2021-10-28T04:00:00"
        ///     }
        /// </remarks>
        public override ValueTask<VRGameRecordHandlerResult> GameRecordAsync(
            [FromServices] IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> service,
            VRGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
