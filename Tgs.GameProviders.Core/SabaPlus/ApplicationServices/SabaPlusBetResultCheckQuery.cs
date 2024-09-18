using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public record SabaPlusBetResultCheckQuery
{
	[Required(ErrorMessage = "Please enter GameId")]
	public int GameId { get; init; } = default!;

	[Required(ErrorMessage = "Please enter ExternalRoundId")]
	public string ExternalRoundId { get; set; }

	[Required(ErrorMessage = "Please enter PlayerCode")]
	public string PlayerCode { get; init; } = default!;
}
