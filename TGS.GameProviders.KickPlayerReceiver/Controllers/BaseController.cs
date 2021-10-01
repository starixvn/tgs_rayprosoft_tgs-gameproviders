using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TCommand> : ControllerBase
    {
        [HttpPost]
        public abstract ValueTask KickPlayerAsync(
            [FromServices] ICommandService<TCommand> commandService,
            TCommand command);
    }
}
