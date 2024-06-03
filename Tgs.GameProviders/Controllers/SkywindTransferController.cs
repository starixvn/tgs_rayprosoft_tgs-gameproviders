using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/swtf")]
public class SkywindTransferController : ControllerBase
{
	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	[HttpPost("gameurl")]
	public async ValueTask<GameUrlResponse?> GameUrlAsync(
		[FromServices] IGetService<SkywindTransferGameUrlQuery, Uri> service,
		GameUrlRequest request)
	{
		var result = await service.GetAsync(new SkywindTransferGameUrlQuery()
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
		}).ConfigureAwait(false);
		return result == null ? null : new GameUrlResponse() { Url = result };
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
		 [FromServices] IGetService<SkywindTransferPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new SkywindTransferPlayerBalanceQuery
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
		[FromServices] IGetService<SkywindTransferPlayerFundCheckingQuery, bool> service,
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
		[FromServices] IGetService<SkywindTransferPlayerFundInQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<SkywindTransferPlayerFundOutQuery, PlayerFundResult> service,
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
		[FromServices] IGetService<SkywindTransferRoundDetailQuery, RoundDetailResult> service,
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
