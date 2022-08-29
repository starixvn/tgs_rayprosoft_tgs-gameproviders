using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IMESportsIsResettled
{
    NotResettled = 0,
    Resettled = 1
}
