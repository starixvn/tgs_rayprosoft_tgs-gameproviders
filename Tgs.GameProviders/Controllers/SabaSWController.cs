using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
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

	[HttpGet("player/{playerCode}/tickets/{refId}")]
	public async ValueTask<GameUrlResponse> TicketDetailAsync(
	[FromServices] IGetService<SabaSWTicketDetailUrlQuery, string> service,
	string playerCode,
	long refId,
	[FromQuery] PlayerTicketDetailRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				PlayerCode = playerCode,
				RefId = refId,
				Language = request.Language,
				BaseCurrency = request.BaseCurrency,
				PlayerUsername = request.PlayerUsername
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameUrlResponse() { Url = new(result) };
	}
}
