using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/habatf")]
public class HabaneroTransferController : ControllerBase
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
	///             "gameType": 15,
	///             "gameId": 5000001,
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
	public async ValueTask<GameUrlResponse> GameUrlAsync(
		[FromServices] IGetService<HabaneroTransferGameUrlQuery, Uri> service,
		GameUrlRequest request)
	{
		var query = new HabaneroTransferGameUrlQuery()
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
		};

		var gameUrl = await service.GetAsync(query).ConfigureAwait(false);

		return new GameUrlResponse()
		{
			Url = gameUrl
		};
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
		[FromServices] ICommandService<HabaneroTransferKickPlayerCommand> commandService,
		string playerCode)
		=> commandService.ExecuteAsync(
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
	/// <remarks>
	///   <b>Example</b><br />
	///
	///   Get balance/4ucmqplxb029
	/// </remarks>
	[HttpGet("balance/{playerCode}")]
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		 [FromServices] IGetService<HabaneroTransferPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new HabaneroTransferPlayerBalanceQuery
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
	///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86"
	///         }
	/// </remarks>
	[HttpPost("playerfundchecking")]
	public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
		[FromServices] IGetService<HabaneroTransferPlayerFundCheckingQuery, bool> service,
		PlayerFundCheckingRequest request)
	{
		var query = new HabaneroTransferPlayerFundCheckingQuery()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId
		};
		var result = await service.GetAsync(query).ConfigureAwait(false);

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
		[FromServices] IGetService<HabaneroTransferPlayerFundInQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<HabaneroTransferPlayerFundOutQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<HabaneroTransferRoundDetailQuery, RoundDetailResult> service,
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
