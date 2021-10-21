using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.TCG.Models
{
    public record TCGUpdatePasswordPlayerRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public string Method => "up";
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; init; }
    }
}
