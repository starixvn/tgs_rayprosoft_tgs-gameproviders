using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.MicroGaming.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/mg")]
[ApiController]
public class MicroGamingController : ControllerBase
{
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse?> GameUrlAsync(
		[FromServices] IGetService<MicroGamingGameUrlQuery, Uri> service,
		GameUrlRequest request)
	{
		var result = await service.GetAsync(
				query: new MicroGamingGameUrlQuery
				{
					GameType = request.GameType,
					GameId = request.GameId,
					BrandCode = request.BrandCode,
					PlayerCode = request.PlayerCode,
					CurrencyCode = request.CurrencyCode,
					LoginRecordId = request.LoginRecordId,
					PlatformType = request.PlatformType,
					Language = request.Language,
					IPAddress = request.IPAddress,
					BackUrl = request.BackUrl
				},
				cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		return new GameUrlResponse { Url = result };
	}

	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		[FromServices] IGetService<MicroGamingPlayerBalanceQuery, PlayerBalanceResult> service,
		string playerCode)
	{
		var result = await service.GetAsync(
			query: new MicroGamingPlayerBalanceQuery
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

	[HttpGet("player/{playerCode}/fund/checking")]
	public async ValueTask<PlayerFundCheckingResponse> PlayerFundCheckingAsync(
		[FromServices] IGetService<MicroGamingPlayerFundCheckingQuery, bool> service,
		string playerCode,
		[FromQuery] PlayerFundCheckingRequest request)
	{
		var result = await service.GetAsync(
			query: new MicroGamingPlayerFundCheckingQuery
			{
				PlayerCode = playerCode,
				OrderId = request.OrderId,
				OrderOn = request.OrderOn
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundCheckingResponse
		{
			OrderStatus = result,
		};
	}

	[HttpPost("player/{playerCode}/fund/in")]
	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
		[FromServices] IGetService<MicroGamingPlayerFundInQuery, PlayerFundResult> service,
		string playerCode,
		PlayerFundRequest request)
	{
		var result = await service.GetAsync(
			query: new MicroGamingPlayerFundInQuery
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

	[HttpPost("player/{playerCode}/fund/out")]
	public async ValueTask<PlayerFundResponse> PlayerFundOutAsync(
		[FromServices] IGetService<MicroGamingPlayerFundOutQuery, PlayerFundResult> service,
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

	[HttpGet("round/{gameBetId:guid}/detail")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(
		[FromServices] IGetService<MicroGamingRoundDetailQuery, RoundDetailResult> service,
		Guid gameBetId,
		[FromQuery] RoundDetailRequest request)
		=> service.GetAsync(
			query: new MicroGamingRoundDetailQuery
			{
				GameBetId = gameBetId,
				Language = request.Language,
				ExternalCreatedOn = request.ExternalCreatedOn
			},
			cancellationToken: HttpContext.RequestAborted);
}
