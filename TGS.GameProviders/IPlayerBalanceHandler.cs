using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders
{
    public interface IPlayerBalanceHandler<TQuery> where TQuery : PlayerBalanceQuery
    {
        ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync([FromServices] IGetService<TQuery, PlayerBalanceResult> getService, PlayerBalanceRequest request);
    }
}
