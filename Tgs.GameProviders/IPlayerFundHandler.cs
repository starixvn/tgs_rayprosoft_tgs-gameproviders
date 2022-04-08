using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders
{
    public interface IPlayerFundHandler<TFundInQuery, TFundOutQuery> where TFundInQuery : PlayerFundQuery where TFundOutQuery : PlayerFundQuery
    {
        [HttpPost("PlayerFundIn")]
        ValueTask<PlayerFundResponse?> PlayerFundInAsync([FromServices] IGetService<TFundInQuery, PlayerFundResult> getService, PlayerFundRequest request);

        [HttpPost("PlayerFundOut")]
        ValueTask<PlayerFundResponse?> PlayerFundOutAsync([FromServices] IGetService<TFundOutQuery, PlayerFundResult> getService, PlayerFundRequest request);
    }
}
