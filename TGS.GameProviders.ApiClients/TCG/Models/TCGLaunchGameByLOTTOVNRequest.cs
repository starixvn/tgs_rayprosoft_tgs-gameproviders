using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.TCG.Models;

namespace TGS.GameProviders.ApiClients.TCG.Models
{
    public record TCGLaunchGameByLOTTOVNRequest : TCGLaunchGameRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType => 420;
        [Required]
        [JsonPropertyName("lottery_bet_mode")]
        public string LotteryBetMode { get; init; } = "Elott";
        [Required]
        [JsonPropertyName("view")]
        public string View { get; init; } = "Lobby";
    }
}
