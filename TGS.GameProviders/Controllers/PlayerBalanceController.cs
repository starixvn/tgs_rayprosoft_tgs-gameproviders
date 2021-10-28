using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/balance/[controller]")]
    public abstract class PlayerBalanceController<TQuery> : ControllerBase where TQuery : PlayerBalanceQuery
    {
        [HttpPost]
        public abstract ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(
            [FromServices] IGetService<TQuery, PlayerBalanceResult> getService,
            PlayerBalanceRequest request);
    }
}
