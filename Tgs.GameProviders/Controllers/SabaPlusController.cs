using Microsoft.AspNetCore.Mvc;
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
		string provider,
		SabaPlusGameUrlQuery source)
	{
		var providerCode = $"{GameProviderType.SabaPlus}-{provider}".ToUpper();

		source.ProviderCode = providerCode;

		var result = await service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameUrlResponse() { Url = result.Url };
	}
}
