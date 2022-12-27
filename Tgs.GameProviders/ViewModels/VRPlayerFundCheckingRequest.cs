using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.ViewModels;

public record VRPlayerFundCheckingRequest
{
    [Required(ErrorMessage = "Please enter BrandCode")]
    public string BrandCode { get; init; } = default!;
    [Required(ErrorMessage = "Please enter PlayerCode")]
    public string PlayerCode { get; init; } = default!;
    [Required(ErrorMessage = "Please enter CurrencyCode")]
    public string CurrencyCode { get; init; } = default!;
    [Required(ErrorMessage = "Please enter OrderId")]
    public string OrderId { get; set; } = default!;
    [Required(ErrorMessage = "Please enter StartTime")]
    public DateTimeOffset StartTime { get; init; } = default!;
    [Required(ErrorMessage = "Please enter EndTime")]
    public DateTimeOffset EndTime { get; init; } = default!;
}
