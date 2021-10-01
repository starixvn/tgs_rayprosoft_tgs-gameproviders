using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.Ky;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class KYGameRecordController : BaseController<KYGameRecordHandlerQuery, KYGameRecordHandlerResult>
    {
        public override ValueTask<KYGameRecordHandlerResult> KickPlayerAsync([FromServices] IGetService<KYGameRecordHandlerQuery, KYGameRecordHandlerResult> service, KYGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
