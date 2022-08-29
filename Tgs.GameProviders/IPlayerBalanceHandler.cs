using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IPlayerBalanceHandler<TQuery> where TQuery : PlayerBalanceQuery
{
    [HttpGet("Balance/{playerCode}")]
    ValueTask<decimal?> PlayerBalanceAsync([FromServices] IGetService<TQuery, decimal?> getService, string playerCode);
}
