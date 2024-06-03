using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tgs.GameProviders.ViewModels;

public record PlayerFundRequest
{
	public string PlayerCode { get; init; } = default!;
	[Required(ErrorMessage = "Please enter OrderId")]
	public string OrderId { get; set; } = default!;
	[Required(ErrorMessage = "Please enter Amount")]
	[Column(TypeName = "decimal(12, 2)")]
	[Range(typeof(decimal), "0", "10000000")]
	public decimal Amount { get; init; } = default!;
}
