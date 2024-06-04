using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/sm01")]
public class SM01Controller : ControllerBase
{
	/// <summary>
	/// Platforms the login asynchronous.
	/// </summary>
	/// <param name="getService">The get service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	///   <b>Example</b><br />
	///
	///   POST
	///     {
	///         "playerCode": "4ucmqplxb029"
	///     }
	/// </remarks>
	[HttpPost("gameurl")]
	public async ValueTask<SM01PlatformLoginResponse?> PlatformLoginAsync(
		[FromServices] IGetService<SM01PlatformLoginQuery, SM01PlatformLoginInfo> getService,
		GameSM01PlatformLoginRequest request)
	{
		var query = new SM01PlatformLoginQuery
		{
			PlayerCode = request.PlayerCode
		};

		var result = await getService
			.GetAsync(query)
			.ConfigureAwait(false);

		return result == null
			? null
			: new SM01PlatformLoginResponse(
				result.ApiUrl,
				result.MerchantId,
				result.CustomerName,
				result.Timestamp,
				result.Sign);
	}

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		[FromServices] IGetService<SM01PlayerBalanceQuery, PlayerBalanceResult> service,
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
		[FromServices] IGetService<SM01PlayerFundInQuery, PlayerFundResult> service,
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
	/// Players the fund out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("player/{playerCode}/fund/out")]
	public async ValueTask<PlayerFundResponse> PlayerFundOutAsync(
		[FromServices] IGetService<SM01PlayerFundOutQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<SM01RoundDetailQuery, RoundDetailResult> service,
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
