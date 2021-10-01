using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.RedisRepositories.Models
{
    public record GameProviderScheduleLockData
    {
        [JsonPropertyName("gameProviderId")]
        public Guid GameProviderId { get; init; }
        [JsonPropertyName("gameProviderCode")]
        public string GameProviderCode { get; init; }
        [JsonPropertyName("gameProviderScheduleId")]
        public Guid GameProviderScheduleId { get; init; }
        [JsonPropertyName("lockDate")]
        public DateTimeOffset LockDate { get; init; }
    }
}
