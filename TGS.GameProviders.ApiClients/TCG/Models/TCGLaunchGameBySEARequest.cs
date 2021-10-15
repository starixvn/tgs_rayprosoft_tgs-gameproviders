using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.TCG.Models.Commons;
using TGS.GameProviders.ApiClients.TCG.Models;

namespace TGS.GameProviders.ApiClients.TCG.Models
{
    public record TCGLaunchGameBySEARequest : TCGLaunchGameRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType => 384;
        [Required]
        [JsonPropertyName("lottery_bet_mode")]
        public string LotteryBetMode { get; init; } = "SEA_Tradition";
        [Required]
        [JsonPropertyName("view")]
        public string View { get; init; } = "Lobby";
        [Required]
        [JsonPropertyName("series")]
        public TCGSEASeries[] series { get; init; } = new TCGSEASeries[] {
            new TCGSEASeries{
                GameGroupCode="MAS",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="VNC",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="THAI",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="SGC",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="LAO",
                PrizeModeId=1
            }
        };
    }
}
