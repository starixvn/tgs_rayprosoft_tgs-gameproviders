using System.ComponentModel;

namespace Tgs.GameProviders.ViewModels;

public record PlayerTicketDetailRequest
{
	[DefaultValue(typeof(string), "en-US")]
	public string? Language { get; set; }

	[DefaultValue(typeof(long?), null)]
	public long? BaseCurrency { get; set; }

	[DefaultValue(typeof(string), null)]
	public string? PlayerUsername { get; set; }
}
