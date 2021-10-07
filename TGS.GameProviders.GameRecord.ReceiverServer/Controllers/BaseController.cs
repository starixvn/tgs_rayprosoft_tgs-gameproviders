using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TQuery, TResult> : ControllerBase
    {
        [HttpPost]
        public abstract ValueTask<TResult> KickPlayerAsync(
            [FromServices] IGetService<TQuery, TResult> service,
            TQuery query);
    }
}
