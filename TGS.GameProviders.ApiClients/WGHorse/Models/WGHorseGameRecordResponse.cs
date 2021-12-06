using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;
using TGS.GameProviders.Core.WGHorse.Models;

namespace TGS.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorseGameRecordResponse : WGHorseResponse
    {
        [JsonPropertyName("Page")]
        public string Page { get; set; }
        [JsonPropertyName("PageTotal")]
        public string PageTotal { get; set; } = default!;
        [JsonPropertyName("PageSize")]
        public string PageSize { get; set; } = default!;
        [JsonPropertyName("RaceDateFrom")]
        public string RaceDateFrom { get; set; } = default!;
        [JsonPropertyName("RaceDateTo")]
        public string RaceDateTo { get; set; } = default!;
        [JsonPropertyName("ArrayInvoice")]
        public WGHorseGameRecord[]? ArrayInvoice { get; set; }
    }
}
