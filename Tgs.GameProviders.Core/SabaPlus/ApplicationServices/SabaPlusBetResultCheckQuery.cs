using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tgs.GameInterface.SabaPlus.Domains.BetResultCheck;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public record SabaPlusBetResultCheckQuery
{
	[JsonIgnore]
	public string GameProviderCode { get; set; } = default!;

	[Required(ErrorMessage = "Please enter ExternalGameId")]
	public string ExternalGameId { get; init; } = default!;

	[Required(ErrorMessage = "Please enter ExternalRoundId")]
	public string ExternalRoundId { get; set; }

	[Required(ErrorMessage = "Please enter PlayerCode")]
	public string PlayerCode { get; init; } = default!;
}

public record SabaPlusBetResultCheckResult
{
	public BetResultCheckResult Result { get; set; } = default!;

	public BetResultCheckQuery Query { get; set; } = default!;
}
