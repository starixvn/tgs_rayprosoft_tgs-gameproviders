using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SabaPlus.Domains.GetGameUrl;
using Tgs.GameProviders.Core.SabaPlus.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
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
	public ValueTask<SabaPlusBetResultCheckResult> BetResultCheck(
	[FromServices] IGetService<SabaPlusBetResultCheckQuery, SabaPlusBetResultCheckResult> service,
	string provider,
	SabaPlusBetResultCheckQuery source)
	{
		var providerCode = $"{GameProviderType.SabaPlus}-{provider}".ToUpper();

		source.GameProviderCode = providerCode;

		return service.GetAsync(query: source, cancellationToken: HttpContext.RequestAborted);
	}

	[HttpPost("ticketdetail")]
	public ValueTask<RoundDetailResult> TicketDetail(
	[FromServices] IGetService<SabaPlusBetHistoryUrlQuery, RoundDetailResult> service,
	string provider,
	SabaPlusBetHistoryUrlQuery source)
	{
		var providerCode = $"{GameProviderType.SabaPlus}-{provider}".ToUpper();

		source.GameProviderCode = providerCode;

		return service.GetAsync(query: source, cancellationToken: HttpContext.RequestAborted);
	}
}
