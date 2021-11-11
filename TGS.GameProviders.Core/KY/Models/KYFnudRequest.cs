using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TGS.GameProviders.Core.KY.Models.Commons;

namespace TGS.GameProviders.Core.KY.Models
{
    public record KYFnudRequest : KYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; set; }

        [JsonPropertyName("money")]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; set; }

        [JsonPropertyName("orderid")]
        public string OrderId { get; set; }
    }
}
