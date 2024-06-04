using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/awc_aws")]
[ApiController]
public class AwcAwsController : ControllerBase
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
		[FromServices] IQueryService<AwcAwsBalancesQuery, BalanceResult> service,
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
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		[FromServices] IGetService<AwcAwsPlayerBalanceQuery, PlayerBalanceResult> service,
		string playerCode)
	{
		var result = await service.GetAsync(
			query: new()
			{
				PlayerCode = playerCode
			},
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
	[HttpPost("player/{playerCode}/fund/in")]
	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
		[FromServices] IGetService<AwcAwsPlayerFundInQuery, PlayerFundResult> service,
		string playerCode,
		PlayerFundRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				PlayerCode = playerCode,
				OrderId = request.OrderId,
				Amount = request.Amount
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundResponse(
			OrderId: result.OrderId,
			ExternalOrderId: result.ExternalOrderId);
	}

	/// <summary>
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpGet("round/{gameBetId:guid}/detail")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(
		[FromServices] IGetService<AwcAwsRoundDetailQuery, RoundDetailResult> service,
		Guid gameBetId,
		[FromQuery] RoundDetailRequest request)
		=> service.GetAsync(
			query: new()
			{
				GameBetId = gameBetId,
				Language = request.Language,
				ExternalCreatedOn = request.ExternalCreatedOn
			},
			cancellationToken: HttpContext.RequestAborted);
}
