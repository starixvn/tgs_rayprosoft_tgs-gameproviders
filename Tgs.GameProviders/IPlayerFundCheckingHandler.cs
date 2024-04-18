using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IPlayerFundCheckingHandler<TQuery, TRequest> where TQuery : PlayerFundCheckingQuery
{
	[HttpPost("playerfundchecking")]
	ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync([FromServices] IGetService<TQuery, bool> getService, TRequest request);
}
