using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGHorse.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorseGameRecordResponse : WGHorseResponse
    {
        [JsonPropertyName("Page")]
        public string Page { get; set; }
        [JsonPropertyName("PageTotal")]
        public string PageTotal { get; set; }
        [JsonPropertyName("PageSize")]
        public string PageSize { get; set; }
        [JsonPropertyName("RaceDateFrom")]
        public string RaceDateFrom { get; set; }
        [JsonPropertyName("RaceDateTo")]
        public string RaceDateTo { get; set; }
        [JsonPropertyName("ArrayInvoice")]
        public WGHorseGameRecord[] ArrayInvoice { get; set; }
    }
}
