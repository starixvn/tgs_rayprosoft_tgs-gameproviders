using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Skywind.Models;

namespace TGS.GameProviders.ApiClients.TCG.Models
{
    public record TCGKickOutLottoPlayerRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public string Method => "kom";
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
    }
}
