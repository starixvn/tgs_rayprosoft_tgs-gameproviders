using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IMESportsIsSettled
    {
        NotSettled = 0,
        Settled = 1
    }
}
