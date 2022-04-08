using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IMESportsIsCancelled
    {
        NotCancel = 0,
        Cancel = 1
    }
}
