using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.ViewModels;

public record PlayerBalanceRequest
{
	[Required(ErrorMessage = "Please enter BrandCode")]
	public string BrandCode { get; init; } = default!;
	[Required(ErrorMessage = "Please enter PlayerCode")]
	public string PlayerCode { get; init; } = default!;
	[Required(ErrorMessage = "Please enter CurrencyCode")]
	public string CurrencyCode { get; init; } = default!;
}
