using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BetConstruct.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BetSelectionState
    {
        NotResulted = 0,
        Returned = 2,
        Lost = 3,
        Won = 4,
        WinReturn = 5,
        LossReturn = 6
    }
}
