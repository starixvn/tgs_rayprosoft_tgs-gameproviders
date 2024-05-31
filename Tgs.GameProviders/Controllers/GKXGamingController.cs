using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.GKXGaming.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/GKX")]
public class GKXGamingController : ControllerBase
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
	///             "gameType": 8,
	///             "gameId": 2000001,
	///             "brandCode": "GPI_PHP",
	///             "playerCode": "4uh3pdl0lcii",
	///             "currencyCode": "PHP",
	///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	///             "platformType": 0,
	///             "language": "zh-CN",
	///             "ipAddress": "127.0.0.1",
	///             "backUrl": "https://dev-t1.rpgp.cc/"
	///         }
	/// </remarks>
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse?> GameUrlAsync(
		[FromServices] IGetService<GKXGamingGameUrlQuery, string> service,
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
	[HttpDelete("kickplayer/{playerCode}")]
	[HttpDelete("player/{playerCode}/logout")]
	public ValueTask KickPlayerAsync(
		[FromServices] ICommandService<GKXGamingKickPlayerCommand> service,
		string playerCode)
	{
		var command = new GKXGamingKickPlayerCommand
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
	///   Get balance/4uh3pdl0lcii
	/// </remarks>
	[HttpGet("balance/{playerCode}")]
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		 [FromServices] IGetService<GKXGamingPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new GKXGamingPlayerBalanceQuery
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
	///             "brandCode": "GPI_PHP",
	///             "playerCode": "4uh3pdl0lcii",
	///             "currencyCode": "PHP",
	///             "orderId":"72412202110271546265054uh3pdl0lcii"
	///         }
	/// </remarks>
	[HttpPost("playerfundchecking")]
	public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
		[FromServices] IGetService<GKXGamingPlayerFundCheckingQuery, bool> service,
		PlayerFundCheckingRequest request)
	{
		var query = new GKXGamingPlayerFundCheckingQuery()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
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
	[HttpPost("playerfundin")]
	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
		[FromServices] IGetService<GKXGamingPlayerFundInQuery, PlayerFundResult> service,
		PlayerFundRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				BrandCode = request.BrandCode,
				PlayerCode = request.PlayerCode,
				CurrencyCode = request.CurrencyCode,
				OrderId = request.OrderId,
				Amount = request.Amount
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundResponse(
			OrderId: request.OrderId,
			GameProviderTransactionId: result.GameProviderTransactionId,
			ExternalGameProviderTransactionId: result.ExternalGameProviderTransactionId);
	}

	/// <summary>
	/// Players the fund out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("playerfundout")]
	public async ValueTask<PlayerFundResponse> PlayerFundOutAsync(
		[FromServices] IGetService<GKXGamingPlayerFundOutQuery, PlayerFundResult> service,
		PlayerFundRequest request)
	{
		var result = await service.GetAsync(
			query: new()
			{
				BrandCode = request.BrandCode,
				PlayerCode = request.PlayerCode,
				CurrencyCode = request.CurrencyCode,
				OrderId = request.OrderId,
				Amount = request.Amount,
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerFundResponse(
			OrderId: request.OrderId,
			GameProviderTransactionId: result.GameProviderTransactionId,
			ExternalGameProviderTransactionId: result.ExternalGameProviderTransactionId);
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
		[FromServices] IGetService<GKXGamingRoundDetailQuery, RoundDetailResult> service,
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
