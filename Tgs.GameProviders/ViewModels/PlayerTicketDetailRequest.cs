using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tgs.GameProviders.ViewModels;

public record PlayerTicketDetailRequest
{
	[Required(ErrorMessage = "Please enter Language")]
	[DefaultValue(typeof(string), "en-US")]
	public string Language { get; set; } = default!;

	[DefaultValue(typeof(long?), null)]
	public long? BaseCurrency { get; set; }

	[DefaultValue(typeof(string), null)]
	public string? PlayerUsername { get; set; }
}
