using System;
using System.ComponentModel.DataAnnotations;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.ViewModels
{
    public record GameUrlRequest
    {
        [Required(ErrorMessage = "Please enter GameType")]
        public GameType GameType { get; init; } = default!;
        [Required(ErrorMessage = "Please enter GameId")]
        public int GameId { get; init; } = default!;
        [Required(ErrorMessage = "Please enter BrandCode")]
        public string BrandCode { get; init; } = default!;
        [Required(ErrorMessage = "Please enter PlayerCode")]
        public string PlayerCode { get; init; } = default!;
        [Required(ErrorMessage = "Please enter CurrencyCode")]
        public string CurrencyCode { get; init; } = default!;
        [Required(ErrorMessage = "Please enter LoginRecordId")]
        public Guid LoginRecordId { get; init; } = default!;
        [Required(ErrorMessage = "Please enter PlatformType")]
        public PlatformType PlatformType { get; init; } = PlatformType.Desktop;
        [Required(ErrorMessage = "Please enter Language")]
        public string Language { get; init; } = default!;
        [Required(ErrorMessage = "Please enter IPAddress")]
        public string IPAddress { get; init; } = default!;
        public Uri? BackUrl { get; init; }
    }
}
