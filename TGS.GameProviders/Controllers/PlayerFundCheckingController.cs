using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/PlayerFundChecking/[controller]")]
    public abstract class PlayerFundCheckingController<TQuery> : ControllerBase where TQuery : PlayerFundCheckingQuery
    {
        [HttpPost]
        public abstract ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
            [FromServices] IGetService<TQuery, PlayerFundCheckingResult> getService,
            PlayerFundCheckingRequest request);
    }
}
