using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.VR;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class VRGameRecordController : BaseController<VRGameRecordHandlerQuery, VRGameRecordHandlerResult>
    {
        public override ValueTask<VRGameRecordHandlerResult> KickPlayerAsync([FromServices] IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> service, VRGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
