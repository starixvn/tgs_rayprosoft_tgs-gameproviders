using System.ComponentModel.DataAnnotations;

namespace TGS.GameProviders.ViewModels
{
    public class KickPlayerRequest
    {
        [Required(ErrorMessage = "Please enter BrandCode")]
        public string BrandCode { get; init; }

        [Required(ErrorMessage = "Please enter CurrencyCode")]
        public string CurrencyCode { get; init; }

        [Required(ErrorMessage = "Please enter PlayerCode")]
        public string PlayerCode { get; init; }
    }
}
