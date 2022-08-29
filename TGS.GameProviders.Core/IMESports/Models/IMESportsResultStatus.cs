using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models;

[JsonConverter(typeof(IMESportsResultStatusConverter))]
public enum IMESportsResultStatus
{
    None = 0,
    Win = 1,
    Lose = 2,
    Draw = 3,
    Unknown = 99,
}
