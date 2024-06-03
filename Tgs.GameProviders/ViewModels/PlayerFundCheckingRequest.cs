using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.ViewModels;

public record PlayerFundCheckingRequest
{
	[Required(ErrorMessage = "Please enter OrderId")]
	public string OrderId { get; set; } = default!;
	[Required(ErrorMessage = "Please enter OrderOn")]
	public DateTimeOffset OrderOn { get; set; } = default!;
}
