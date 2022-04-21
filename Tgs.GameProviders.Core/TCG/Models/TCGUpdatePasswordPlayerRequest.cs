using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.TCG.Models
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
