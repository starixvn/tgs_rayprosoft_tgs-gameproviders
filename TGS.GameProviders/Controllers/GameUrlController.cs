using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/gameurl/[controller]")]
    public abstract class GameUrlController<TQuery> : ControllerBase where TQuery : GameUrlQuery
    {
        [HttpPost]
        public abstract ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<TQuery, GameUrlResult> getService,
            GameUrlRequest request);
    }
}
