using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/365tf")]
[ApiController]
public class Cards365TransferController : ControllerBase
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
		[FromServices] IGetService<Cards365TransferGameUrlQuery, Uri> getService,
		GameUrlRequest request)
	{
		var result = await getService.GetAsync(
				query: new Cards365TransferGameUrlQuery()
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
		 [FromServices] IGetService<Cards365TransferPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new Cards365TransferPlayerBalanceQuery
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
	/// <param name="getService">The get service.</param>
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
	///             "orderId":"7a7147218501000000010000af506b2c"
	///         }
	/// </remarks>
	[HttpPost("playerfundchecking")]
	public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
		[FromServices] IGetService<Cards365TransferPlayerFundCheckingQuery, bool> getService,
		PlayerFundCheckingRequest request)
	{
		var result = await getService.GetAsync(new()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
		}).ConfigureAwait(false);

		return new PlayerFundCheckingResponse()
		{
			OrderStatus = result,
		};
	}

	/// <summary>
	/// Players the fund in asynchronous.
	/// </summary>
	/// <param name="getService">The get service.</param>
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
	///             "orderId":"7a3f35bf271247b998f6750d448ed757",
	///             "amount": 100
	///         }
	/// </remarks>
	[HttpPost("playerfundin")]
	public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
		[FromServices] IGetService<Cards365TransferPlayerFundInQuery, PlayerFundResult> getService,
		PlayerFundRequest request)
	{
		var result = await getService.GetAsync(new()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
			Amount = request.Amount,
		}).ConfigureAwait(false);
		return result == null ? null : new PlayerFundResponse()
		{
			OrderId = result.OrderId
		};
	}

	/// <summary>
	/// Players the fund out asynchronous.
	/// </summary>
	/// <param name="getService">The get service.</param>
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
	///             "orderId":"7a7147218501000000010000af506b2c",
	///             "amount": 1000
	///         }
	/// </remarks>
	[HttpPost("playerfundout")]
	public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
		[FromServices] IGetService<Cards365TransferPlayerFundOutQuery, PlayerFundResult> getService,
		PlayerFundRequest request)
	{
		var result = await getService.GetAsync(new()
		{
			BrandCode = request.BrandCode,
			PlayerCode = request.PlayerCode,
			CurrencyCode = request.CurrencyCode,
			OrderId = request.OrderId,
			Amount = request.Amount,
		}).ConfigureAwait(false);
		return result == null ? null : new PlayerFundResponse()
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
		[FromServices] IGetService<Cards365TransferRoundDetailQuery, RoundDetailResult> service,
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
