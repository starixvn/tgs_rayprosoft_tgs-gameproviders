using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core.SabaSW.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/sabasw")]
public class SabaSWController : ControllerBase
{
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse> GameUrlAsync(
		[FromServices] IGetService<SabaSWGameUrlQuery, string> service,
		SabaSWGameUrlQuery source)
	{
		var result = await service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		return new GameUrlResponse() { Url = new(result) };
	}

	[HttpPost("ticketurl")]
	public async ValueTask<RoundDetailResult> TicketUrlAsync(
	[FromServices] IGetService<SabaSWTicketDetailUrlQuery, RoundDetailResult> service,
	[FromBody] SabaSWTicketDetailUrlQuery source)
	{
		var result = await service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result;
	}
}
