using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.VRGaming.ApplicationServices;

using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/vr")]
public class VRGamingController : ControllerBase
{
	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "gameType": 13,
	///             "gameId": 1100001,
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
		[FromServices] IGetService<VRGamingGameUrlQuery, string> service,
		GameUrlRequest request)
	{
		var result = await service
			.GetAsync(
				query: new()
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
		return result == null ? null : new GameUrlResponse() { Url = new(result) };
	}

	/// <summary>
	/// Kicks the player asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Delete kickplayer/4ucmqplxb029
	/// </remarks>
	[HttpDelete("kickplayer/{playerCode}")]
	public ValueTask KickPlayerAsync(
		[FromServices] ICommandService<VRGamingKickPlayerCommand> service,
		string playerCode)
	{
		var command = new VRGamingKickPlayerCommand
		{
			PlayerCode = playerCode
		};
		return service
		.ExecuteAsync(
			command: command,
			cancellationToken: HttpContext.RequestAborted);
	}

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
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		 [FromServices] IGetService<VRGamingPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new VRGamingPlayerBalanceQuery
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
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "CNY",
	///             "orderId":"018c8f8f201f0000000107e7db655fdb"
	///             "startTime": "2023-12-22T11:27:00+08:00",
	///             "endTime": "2023-12-22T11:42:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("playerfundchecking")]
	public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
		[FromServices] IGetService<VRGamingPlayerFundCheckingQuery, bool> service,
		VRGamingPlayerFundCheckingRequest request)
	{
		var query = new VRGamingPlayerFundCheckingQuery()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
			StartTime = request.StartTime,
			EndTime = request.EndTime
		};
		var result = await service
			.GetAsync(
			query: query,
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
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "CNY",
	///             "orderId":"018c8f8ee6a60000000107e7db655fdb",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("playerfundin")]
	public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
		[FromServices] IGetService<VRGamingPlayerFundInQuery, PlayerFundResult> service,
		PlayerFundRequest request)
	{
		var query = new VRGamingPlayerFundInQuery()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
			Amount = request.Amount,
		};

		var result = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundResponse()
		{
			OrderId = result.OrderId
		};
	}

	/// <summary>
	/// Players the fund out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "CNY",
	///             "orderId":"018c8f8f201f0000000107e7db655fdb",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("playerfundout")]
	public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
		[FromServices] IGetService<VRGamingPlayerFundOutQuery, PlayerFundResult> service,
		PlayerFundRequest request)
	{
		var query = new VRGamingPlayerFundOutQuery()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
			Amount = request.Amount,
		};

		var result = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundResponse()
		{
			OrderId = result.OrderId
		};
	}

	/// <summary>
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	[HttpGet("rounddetail/{gameBetId:guid}")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(
		[FromServices] IGetService<VRGamingRoundDetailQuery, RoundDetailResult> service,
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
