using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core.GTPlaytech.ApplicationServices;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;
using Tgs.GameProviders.Core.GTExpanse.ApplicationServices;

namespace Tgs.GameProviders.Controllers;

[Route("api/GT_PT")]
[ApiController]
public class GTPlaytechController : ControllerBase
{
	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse?> GameUrlAsync(
		[FromServices] IGetService<GTPlaytechGameUrlQuery, string> service,
		GTPlaytechGameUrlQuery source)
	{
		var result = await service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		return new GameUrlResponse() { Url = new Uri(result) };
	}

	/// <summary>
	/// Players the logout asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	[HttpDelete("player/{playerCode}/logout")]
	public ValueTask PlayerLogoutAsync(
		[FromServices] ICommandService<GTPlaytechPlayerLogoutCommand> service,
		string playerCode)
		=> service.ExecuteAsync(
			command: new()
			{
				PlayerCode = playerCode
			},
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		[FromServices] IGetService<GTPlaytechPlayerBalanceQuery, PlayerBalanceResult> service,
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
	/// Players the fund checking asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpGet("player/{playerCode}/fund/checking")]
	public async ValueTask<PlayerFundCheckingResponse> PlayerFundCheckingAsync(
		[FromServices] IGetService<GTPlaytechPlayerFundCheckingQuery, bool> service,
		string playerCode,
		[FromQuery] PlayerFundCheckingRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				PlayerCode = playerCode,
				OrderId = request.OrderId,
				OrderOn = request.OrderOn
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundCheckingResponse()
		{
			OrderStatus = result,
		};
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
		[FromServices] IGetService<GTPlaytechPlayerFundInQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<GTPlaytechPlayerFundOutQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<GTPlaytechRoundDetailQuery, RoundDetailResult> service,
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
