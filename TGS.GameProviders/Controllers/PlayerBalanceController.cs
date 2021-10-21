using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/balance/[controller]")]
    public abstract class PlayerBalanceController<TQuery, TResult> : ControllerBase where TQuery : PlayerBalanceQuery where TResult : PlayerBalanceResult
    {
        [HttpPost]
        public abstract ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
            [FromServices] IGetService<TQuery, TResult> getService,
            PlayerBalanceRequest request);
    }
}
