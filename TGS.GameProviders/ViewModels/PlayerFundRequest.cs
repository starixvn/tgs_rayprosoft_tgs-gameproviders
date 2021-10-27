using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TGS.GameProviders.ViewModels
{
    public record PlayerFundRequest
    {
        [Required(ErrorMessage = "Please enter BrandCode")]
        public string BrandCode { get; init; }
        [Required(ErrorMessage = "Please enter PlayerCode")]
        public string PlayerCode { get; init; }
        [Required(ErrorMessage = "Please enter CurrencyCode")]
        public string CurrencyCode { get; init; }
        [Required(ErrorMessage = "Please enter OrderId")]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "Please enter Amount")]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; init; }
    }
}
