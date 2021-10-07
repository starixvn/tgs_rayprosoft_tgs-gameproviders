using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.VR;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class VRGameRecordController : BaseController<VRGameRecordHandlerQuery, VRGameRecordHandlerResult>
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
        ///         "startTime":"2021-06-28T07:32:53",
        ///         "endTime":"2021-06-30T07:33:53"
        ///     }
        /// </remarks>
        public override ValueTask<VRGameRecordHandlerResult> KickPlayerAsync(
            [FromServices] IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> service,
            VRGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
