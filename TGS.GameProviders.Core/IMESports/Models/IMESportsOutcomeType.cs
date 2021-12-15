using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IMESportsOutcomeType
    {
        Missed = 0,
        Hit = 1
    }
}
