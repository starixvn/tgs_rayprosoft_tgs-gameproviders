using System;
using System.ComponentModel.DataAnnotations;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.ViewModels;

public class EvolutionTransferBalanceRequest
{
    [Required(ErrorMessage = "Please enter CurrencyCode")]
    public string CurrencyCode { get; init; } = default!;

    [Required(ErrorMessage = "Please enter GameId")]
    public int GameId { get; init; } = default!;

    [Required(ErrorMessage = "Please enter IPAddress")]
    public string IpAddress { get; init; } = default!;

    [Required(ErrorMessage = "Please enter Language")]
    public string Language { get; init; } = default!;

    [Required(ErrorMessage = "Please enter LoginRecordId")]
    public Guid LoginRecordId { get; init; } = default!;

    [Required(ErrorMessage = "Please enter PlatformType")]
    public PlatformType PlatformType { get; init; } = PlatformType.Desktop;
}
