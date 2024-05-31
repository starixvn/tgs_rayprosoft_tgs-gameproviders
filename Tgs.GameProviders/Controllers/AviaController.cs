using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AviaController : ControllerBase
{
	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "gameType": 4,
	///             "gameId": 2300001,
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
	public ValueTask<GameUrlResponse?> GameUrlAsync(GameUrlRequest request) => throw new NotImplementedException();

	/// <summary>
	/// Kicks the player asynchronous.
	/// </summary>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	/// <exception cref="System.NotImplementedException"></exception>
	[HttpDelete("kickplayer/{playerCode}")]
	[HttpDelete("player/{playerCode}/logout")]
	public ValueTask KickPlayerAsync(string playerCode) => throw new NotImplementedException();

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	/// <remarks>
	///   <b>Example</b><br />
	///
	///   Get balance/4ucmqplxb029
	/// </remarks>
	[HttpGet("balance/{playerCode}")]
	[HttpGet("player/{playerCode}/balance")]
	public ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string playerCode) => throw new NotImplementedException();

	/// <summary>
	/// Players the fund checking asynchronous.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "HKD",
	///             "orderId":"72412202110271546265054ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("playerfundchecking")]
	public ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(PlayerFundCheckingRequest request) => throw new NotImplementedException();

	/// <summary>
	/// Players the fund in asynchronous.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "HKD",
	///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("playerfundin")]
	public ValueTask<PlayerFundResponse?> PlayerFundInAsync(PlayerFundRequest request) => throw new NotImplementedException();

	/// <summary>
	/// Players the fund out asynchronous.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "currencyCode": "HKD",
	///             "orderId":"D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("playerfundout")]
	public ValueTask<PlayerFundResponse?> PlayerFundOutAsync(PlayerFundRequest request) => throw new NotImplementedException();

	/// <summary>
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	/// <exception cref="System.NotImplementedException"></exception>
	[HttpGet("rounddetail/{gameBetId:guid}")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(Guid gameBetId, [FromQuery] string language) => throw new NotImplementedException();
}
