using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.ViewModels;

public record GameSM01PlatformLoginRequest
{
    [Required(ErrorMessage = "Please enter PlayerCode")]
    public string PlayerCode { get; init; } = default!;
}
