using System.ComponentModel.DataAnnotations;

namespace TGS.GameProviders.ViewModels
{
    public class KickPlayerRequest
    {
        [Required(ErrorMessage = "Please enter BrandCode")]
        public string BrandCode { get; init; } = default!;
        [Required(ErrorMessage = "Please enter PlayerCode")]
        public string PlayerCode { get; init; } = default!;
        [Required(ErrorMessage = "Please enter CurrencyCode")]
        public string CurrencyCode { get; init; } = default!;
    }
}
