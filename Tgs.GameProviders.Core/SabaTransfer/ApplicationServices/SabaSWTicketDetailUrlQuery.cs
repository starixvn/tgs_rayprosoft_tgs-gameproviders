using System.ComponentModel;

namespace Tgs.GameProviders.Core.SabaSW.ApplicationServices;

public record SabaSWTicketDetailUrlQuery
{
	public required string PlayerCode { get; init; }

	public required string ExternalRoundId { get; set; }

	[DefaultValue(typeof(string), "en-US")]
	public string Language { get; init; } = default!;

	[DefaultValue(typeof(string), null)]
	public string BaseCurrency { get; set; }

	[DefaultValue(typeof(string), null)]
	public string PlayerUsername { get; set; }
}
