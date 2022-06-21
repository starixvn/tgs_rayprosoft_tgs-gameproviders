using System.Text.Json;

namespace Tgs.GameProviders.Domains.ConfigurationData
{
    public record AwcConfigurationData(
        string AgentId,
        string Cert,
        JsonDocument? BetLimit);
}
