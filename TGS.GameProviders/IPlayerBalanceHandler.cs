using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders
{
    public interface IPlayerBalanceHandler<TQuery> where TQuery : PlayerBalanceQuery
    {
        [HttpGet("Balance")]
        ValueTask<decimal?> PlayerBalanceAsync([FromServices] IGetService<TQuery, decimal?> getService, [FromQuery] string brandCode, [FromQuery] string playerCode, [FromQuery] string currencyCode);
    }
}
