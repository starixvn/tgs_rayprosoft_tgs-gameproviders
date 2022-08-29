using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.TCG.Models;

public record TCGKickOutLottoPlayerRequest
{
    [Required]
    [JsonPropertyName("method")]
    public string Method => "kom";
    [Required]
    [JsonPropertyName("username")]
    public string UserName { get; init; }
}
