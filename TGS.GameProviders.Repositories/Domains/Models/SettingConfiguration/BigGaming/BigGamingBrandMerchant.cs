using System;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.BigGaming
{
    public record BigGamingBrandMerchant
    {
        public Uri ApiUrl { get; init; }

        public bool Status { get; set; }

        public string CurrencyCode { get; set; }

        public string AgentLoginIdPassword { get; init; }

        public string SecretKey { get; init; }

        public string SecretCode { get; init; }
    }
}
