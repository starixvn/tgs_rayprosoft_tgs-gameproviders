using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders
{
    public interface IPlayerFundHandler<TFundInQuery, TFundOutQuery> where TFundInQuery : PlayerFundQuery where TFundOutQuery : PlayerFundQuery
    {
        ValueTask<PlayerFundResponse?> PlayerFundInAsync([FromServices] IGetService<TFundInQuery, PlayerFundResult> getService, PlayerFundRequest request);

        ValueTask<PlayerFundResponse?> PlayerFundOutAsync([FromServices] IGetService<TFundOutQuery, PlayerFundResult> getService, PlayerFundRequest request);
    }
}
