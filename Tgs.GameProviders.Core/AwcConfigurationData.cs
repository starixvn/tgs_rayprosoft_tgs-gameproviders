using System.Text.Json;

namespace Tgs.GameProviders.Core
{
    public record AwcConfigurationData(
        string AgentId,
        string Cert,
        JsonDocument BetLimit);
}
