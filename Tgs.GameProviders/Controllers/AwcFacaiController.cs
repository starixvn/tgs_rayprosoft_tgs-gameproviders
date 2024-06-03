using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcFacai.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/awc_facai")]
[ApiController]
public class AwcFacaiController : ControllerBase
{
	/// <summary>
	/// Brands the player balances asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="brandCode">The brand code.</param>
	/// <returns></returns>
	/// <remarks>
	///   <b>Example</b><br />
	///
	///   Get GPI/player/balances
	/// </remarks>
	[HttpGet("{brandCode}/player/balances")]
	public IAsyncEnumerable<BalanceResult> BrandPlayerBalancesAsync(
		[FromServices] IQueryService<AwcFacaiBalancesQuery, BalanceResult> service,
		string brandCode)
		=> service.QueryAsync(
			query: new(BrandCode: brandCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	/// <remarks>
	///   <b>Example</b><br />
	///
	///   Get balance/4ucmqplxb029
	/// </remarks>
	[HttpGet("balance/{playerCode}")]
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		 [FromServices] IGetService<AwcFacaiPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new AwcFacaiPlayerBalanceQuery
		{
			PlayerCode = playerCode
		};
		var result = await service
		 .GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
		 .ConfigureAwait(false);

		return new PlayerBalanceResponse(
			result.Balance,
			result.FreeBalance,
			result.OutstandingBalance);
	}

	/// <summary>
	/// Players the fund in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("playerfundin")]
	[HttpPost("player/{playerCode}/fund/in")]
	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
		[FromServices] IGetService<AwcFacaiPlayerFundInQuery, PlayerFundResult> service,
		PlayerFundRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				PlayerCode = request.PlayerCode,
				OrderId = request.OrderId,
				Amount = request.Amount
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundResponse(
			OrderId: result.OrderId,
			ExternalOrderId: result.ExternalOrderId,
			GameProviderTransactionId: result.OrderId,
			ExternalGameProviderTransactionId: result.ExternalOrderId);
	}

	/// <summary>
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	[HttpGet("rounddetail/{gameBetId:guid}")]
	[HttpGet("round/{gameBetId:guid}/detail")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(
		[FromServices] IGetService<AwcFacaiRoundDetailQuery, RoundDetailResult> service,
		Guid gameBetId,
		[FromQuery] string language)
		=> service.GetAsync(
			query: new()
			{
				GameBetId = gameBetId,
				Language = language
			},
			cancellationToken: HttpContext.RequestAborted);
}
