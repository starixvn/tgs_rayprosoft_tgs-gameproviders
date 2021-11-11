using System;

namespace TGS.GameProviders.Domains.SettingConfiguration.BigGaming
{
    public record BigGamingBrandMerchant
    {
        public Uri ApiUrl { get; init; } = default!;

        public bool Status { get; init; }

        public string CurrencyCode { get; init; } = default!;

        public string AgentLoginIdPassword { get; init; } = default!;

        public string SecretKey { get; init; } = default!;

        public string SecretCode { get; init; } = default!;
    }
}
