using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    public abstract class PlayerFundController<TFundInQuery, TFundOutQuery, TResult> : ControllerBase where TFundInQuery : PlayerFundQuery where TFundOutQuery : PlayerFundQuery where TResult : PlayerFundResult
    {
        [HttpPost("api/PlayerFundIn/[controller]")]
        public abstract ValueTask<PlayerFundResponse> PlayerFundInAsync(
            [FromServices] IGetService<TFundInQuery, TResult> getService,
            PlayerFundRequest request);

        [HttpPost("api/PlayerFundOut/[controller]")]
        public abstract ValueTask<PlayerFundResponse> PlayerFundOutAsync(
            [FromServices] IGetService<TFundOutQuery, TResult> getService,
            PlayerFundRequest request);
    }
}
