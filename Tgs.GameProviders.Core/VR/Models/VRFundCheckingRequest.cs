using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models;

public record VRFundCheckingRequest
{
    [JsonPropertyName("startTime")]
    public string StartTime { get; init; }
    [JsonPropertyName("endTime")]
    public string EndTime { get; init; }
    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; init; }
    [JsonPropertyName("playerName")]
    public string PlayerCode { get; init; }
    [JsonPropertyName("recordPage")]
    public int RecordPage { get; init; }
    [JsonPropertyName("recordCountPerPage")]
    public int RecordCountPerPage { get; init; }
}
