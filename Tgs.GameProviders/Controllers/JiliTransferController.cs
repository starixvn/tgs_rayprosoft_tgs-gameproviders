using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/jilitf")]
[ApiController]
public class JiliTransferController : ControllerBase,
		IGameUrlHandler<JiliTransferGameUrlQuery>,
		IKickPlayerHandler<JiliTransferKickPlayerCommand>,
		IPlayerFundHandler<JiliTransferPlayerFundInQuery, JiliTransferPlayerFundOutQuery>,
		IPlayerFundCheckingHandler<JiliTransferPlayerFundCheckingQuery, PlayerFundCheckingRequest>
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
		[FromServices] IGetService<JiliTransferGameUrlQuery, GameUrlResult> getService,
		GameUrlRequest request)
	{
		var result = await getService.GetAsync(new JiliTransferGameUrlQuery()
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
		return result == null ? null : new GameUrlResponse() { Url = result.Url };
	}

	/// <summary>
	/// Kicks the player asynchronous.
	/// </summary>
	/// <param name="commandService">The command service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Delete kickplayer/4ucmqplxb029
	/// </remarks>
	[HttpDelete("kickplayer/{playerCode}")]
	public ValueTask KickPlayerAsync(
		[FromServices] ICommandService<JiliTransferKickPlayerCommand> commandService,
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
	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
		 [FromServices] IGetService<JiliTransferPlayerBalanceQuery, PlayerBalanceResult> service,
		 string playerCode)
	{
		var query = new JiliTransferPlayerBalanceQuery
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
	///             "orderId":"72412202110271546265054ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("playerfundchecking")]
	public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
		[FromServices] IGetService<JiliTransferPlayerFundCheckingQuery, bool> getService,
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
	///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("playerfundin")]
	public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
		[FromServices] IGetService<JiliTransferPlayerFundInQuery, PlayerFundResult> getService,
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
			OrderId = request.OrderId
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
	///             "orderId":"D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("playerfundout")]
	public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
		[FromServices] IGetService<JiliTransferPlayerFundOutQuery, PlayerFundResult> getService,
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
			OrderId = request.OrderId
		};
	}

	/// <summary>
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="getService">The get service.</param>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	[HttpGet("rounddetail/{gameBetId:guid}")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(
		[FromServices] IGetService<JiliTransferRoundDetailQuery, RoundDetailResult> getService,
		Guid gameBetId,
		[FromQuery] string language)
		=> getService.GetAsync(
			query: new()
			{
				GameBetId = gameBetId,
				Language = language
			},
			cancellationToken: HttpContext.RequestAborted);
}
