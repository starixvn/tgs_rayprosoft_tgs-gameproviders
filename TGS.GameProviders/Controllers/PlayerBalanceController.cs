using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.Core;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/balance/[controller]")]
    public abstract class PlayerBalanceController<TQuery, TResult> : ControllerBase where TQuery : PlayerBalanceQuery where TResult : PlayerBalanceResult
    {
        [HttpPost]
        public abstract ValueTask<TResult> PlayerBalanceAsync(
            [FromServices] IGetService<TQuery, TResult> getService,
            TQuery query);
    }
}
