using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders
{
    public interface IPlayerFundCheckingHandler<TQuery, TRequest> where TQuery : PlayerFundCheckingQuery
    {
        [HttpPost("PlayerFundChecking")]
        ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync([FromServices] IGetService<TQuery, PlayerFundCheckingResult> getService, TRequest request);
    }
}
