using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.FaChai.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/fachai")]
[ApiController]
public class FaChaiController : ControllerBase
{
	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="getService">The get service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "gameType": 16,
	///             "gameId": 3000001,
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "CNY",
	///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	///             "platformType": 0,
	///             "language": "zh-CN",
	///             "ipAddress": "127.0.0.1",
	///             "backUrl": "https://dev-t1.rpgp.cc/"
	///         }
	/// </remarks>
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse?> GameUrlAsync(
		[FromServices] IGetService<FaChaiGameUrlQuery, Uri> getService,
		GameUrlRequest request)
	{
		var result = await getService.GetAsync(
				query: new FaChaiGameUrlQuery()
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
		return new GameUrlResponse() { Url = result };
	}

	/// <summary>
	/// Kicks the player asynchronous.
	/// </summary>
	/// <param name="commandService">The command service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	[HttpDelete("kickplayer/{playerCode}")]
	[HttpDelete("player/{playerCode}/logout")]
	public ValueTask KickPlayerAsync(
		[FromServices] ICommandService<FaChaiKickPlayerCommand> commandService,
		string playerCode)
		=> commandService.ExecuteAsync(new()
		{
			PlayerCode = playerCode
		});

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
		 [FromServices] IGetService<FaChaiPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new FaChaiPlayerBalanceQuery
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
	/// Players the fund checking asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpGet("player/{playerCode}/fund/checking")]
	public async ValueTask<PlayerFundCheckingResponse> PlayerFundCheckingAsync(
		[FromServices] IGetService<FaChaiPlayerFundCheckingQuery, bool> service,
		string playerCode,
		[FromQuery] PlayerFundCheckingRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				PlayerCode = playerCode,
				OrderId = request.OrderId,
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
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("playerfundin")]
	[HttpPost("player/{playerCode}/fund/in")]
	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
		[FromServices] IGetService<FaChaiPlayerFundInQuery, PlayerFundResult> service,
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
	/// Players the fund out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("playerfundout")]
	[HttpPost("player/{playerCode}/fund/out")]
	public async ValueTask<PlayerFundResponse> PlayerFundOutAsync(
		[FromServices] IGetService<FaChaiPlayerFundOutQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<FaChaiRoundDetailQuery, RoundDetailResult> service,
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
