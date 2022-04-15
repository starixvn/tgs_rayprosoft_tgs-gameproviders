using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IMESportsIsSettled
    {
        NotSettled = 0,
        Settled = 1
    }
}
