using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SGController : ControllerBase
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
	public ValueTask KickPlayerAsync(string playerCode) => throw new NotImplementedException();

	/// <summary>
	/// Rounds the detail asynchronous.
	/// </summary>
	/// <param name="roundId">The round identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	/// <exception cref="System.NotImplementedException"></exception>
	[HttpGet("rounddetail/{roundId:guid}")]
	public ValueTask<RoundDetailResult?> RoundDetailAsync(Guid roundId, [FromQuery] string language) => throw new NotImplementedException();
}
