using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tgs.GameProviders.Core.SabaSW.ApplicationServices;

public record SabaSWTicketDetailUrlQuery
{
	public required string PlayerCode { get; init; }

	public required long RefId { get; set; }

	[DefaultValue(typeof(string), "en-US")]
	public string Language { get; init; } = default!;

	[DefaultValue(typeof(long?), null)]
	public long? BaseCurrency { get; set; }

	[DefaultValue(typeof(string), null)]
	public string PlayerUsername { get; set; }
}
