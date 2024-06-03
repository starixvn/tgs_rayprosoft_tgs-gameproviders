using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HABAController : ControllerBase
{
	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns></returns>
	/// <exception cref="System.NotImplementedException"></exception>
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
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="gameBetId">The game bet identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	/// <exception cref="System.NotImplementedException"></exception>
	[HttpGet("rounddetail/{gameBetId:guid}")]
	[HttpGet("round/{gameBetId:guid}/detail")]
	public ValueTask<RoundDetailResult> RoundDetailAsync(Guid gameBetId, [FromQuery] string language) => throw new NotImplementedException();
}
