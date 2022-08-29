using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.TF.Models.Commons;

public class LaunchResponse
{
    [JsonPropertyName("launch_url")]
    public string LaunchUrl { get; set; }
}
