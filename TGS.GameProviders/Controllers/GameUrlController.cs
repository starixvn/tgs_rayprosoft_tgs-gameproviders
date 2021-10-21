using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/gameurl/[controller]")]
    public abstract class GameUrlController<TQuery, TResult> : ControllerBase where TQuery : GameUrlQuery where TResult : GameUrlResult
    {
        [HttpPost]
        public abstract ValueTask<GameUrlResponse> GameUrlAsync(
            [FromServices] IGetService<TQuery, TResult> getService,
            GameUrlRequest request);
    }
}
