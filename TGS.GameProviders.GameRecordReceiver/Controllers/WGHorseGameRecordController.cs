using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.WGHorse;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class WGHorseGameRecordController : BaseController<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult>
    {
        public override ValueTask<WGHorseGameRecordHandlerResult> KickPlayerAsync([FromServices] IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult> service, WGHorseGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
