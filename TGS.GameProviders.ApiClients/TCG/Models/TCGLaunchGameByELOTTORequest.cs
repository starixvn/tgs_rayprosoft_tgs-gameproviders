using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGLaunchGameByELOTTORequest : TCGLaunchGameRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType => 2;
        [Required]
        [JsonPropertyName("lottery_bet_mode")]
        public string LotteryBetMode { get; set; } = "Elott_Traditional";
        [Required]
        [JsonPropertyName("view")]
        public string View { get; set; } = "Lobby";
    }
}
