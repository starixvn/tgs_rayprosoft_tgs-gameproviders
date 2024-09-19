using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SabaPlus.Domains.BetResultCheck;
using Tgs.GameInterface.SabaPlus.Domains.GetGameUrl;
using Tgs.GameProviders.Core.SabaPlus.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/sabaplus-{provider}")]
public class SabaPlusController : ControllerBase
{
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse> GameUrlAsync(
		[FromServices] IGetService<SabaPlusGameUrlQuery, GetGameUrlResult> service,
		SabaPlusGameUrlQuery source)
	{
		var result = await service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameUrlResponse() { Url = result.ReturnUrl };
	}

	[HttpPost("betresultcheck")]
	public ValueTask<BetResultCheckResult> BetResultCheck(
	[FromServices] IGetService<SabaPlusBetResultCheckQuery, BetResultCheckResult> service,
	SabaPlusBetResultCheckQuery source)
	 => service.GetAsync(query: source, cancellationToken: HttpContext.RequestAborted);
}
