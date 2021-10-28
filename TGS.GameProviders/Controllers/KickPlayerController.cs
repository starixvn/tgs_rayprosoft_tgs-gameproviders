using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/kickplayer/[controller]")]
    public abstract class KickPlayerController<TCommand> : ControllerBase where TCommand : KickPlayerCommand
    {
        [HttpPost]
        public abstract ValueTask KickPlayerAsync(
            [FromServices] ICommandService<TCommand> commandService,
            KickPlayerRequest request);
    }
}
