// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record SBOCreatePlayerRequest : SBOBasePlayerRequest
    {
        private readonly string _agentPrefix;
        private readonly string _brandCode;
        private readonly string _currencyCode;

        public SBOCreatePlayerRequest(string agentPrefix, string brandCode, string currencyCode)
        {
            _agentPrefix = agentPrefix;
            _brandCode = brandCode;
            _currencyCode = currencyCode;
        }

        /// <summary>
        /// As per SBO, a agent can only have 1 currency
        /// </summary>
        [JsonPropertyName("Agent")]
        public string AgentId =>
            $"{_agentPrefix}{_brandCode.ToUpper()}{_currencyCode.Replace("_", string.Empty).ToUpper()}";
    }
}