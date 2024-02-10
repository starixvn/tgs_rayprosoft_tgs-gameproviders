using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tgs.GameProviders.ViewModels;

public record PlayerFundRequest
{
    [Required(ErrorMessage = "Please enter BrandCode")]
    public string BrandCode { get; init; } = default!;
    [Required(ErrorMessage = "Please enter PlayerCode")]
    public string PlayerCode { get; init; } = default!;
    [Required(ErrorMessage = "Please enter CurrencyCode")]
    public string CurrencyCode { get; init; } = default!;
    [Required(ErrorMessage = "Please enter OrderId")]
    public string OrderId { get; set; } = default!;
    [Required(ErrorMessage = "Please enter Amount")]
    [Column(TypeName = "decimal(12, 2)")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Amount { get; init; } = default!;
}
