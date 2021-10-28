using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    public abstract class PlayerFundController<TFundInQuery, TFundOutQuery> : ControllerBase where TFundInQuery : PlayerFundQuery where TFundOutQuery : PlayerFundQuery
    {
        [HttpPost("api/PlayerFundIn/[controller]")]
        public abstract ValueTask<PlayerFundResponse?> PlayerFundInAsync(
            [FromServices] IGetService<TFundInQuery, PlayerFundResult> getService,
            PlayerFundRequest request);

        [HttpPost("api/PlayerFundOut/[controller]")]
        public abstract ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
            [FromServices] IGetService<TFundOutQuery, PlayerFundResult> getService,
            PlayerFundRequest request);
    }
}
