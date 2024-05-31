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
	/// <remarks>
	///   <b>Example</b><br />
	///
	///   Get balance/4ucmqplxb029
	/// </remarks>
	[HttpGet("balance/{playerCode}")]
	[HttpGet("player/{playerCode}/balance")]
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		 [FromServices] IGetService<SM01PlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new SM01PlayerBalanceQuery
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
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("playerfundin")]
	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
		[FromServices] IGetService<SM01PlayerFundInQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<SM01PlayerFundOutQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<SM01RoundDetailQuery, RoundDetailResult> service,
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
