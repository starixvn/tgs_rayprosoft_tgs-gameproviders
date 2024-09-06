using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core.SabaPlus.ApplicationServices;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/sabaplus-{provider}")]
public class SabaPlusController : ControllerBase
{
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse> GameUrlAsync(
		[FromServices] IGetService<SabaPlusGameUrlQuery, string> service,
		SabaPlusGameUrlQuery source)
	{
		var result = await service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameUrlResponse() { Url = new(result) };
	}
}
