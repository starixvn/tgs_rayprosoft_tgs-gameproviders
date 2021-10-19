using System;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.VR
{
    public record VRBrandMerchant
    {
        public string BrandCode { get; init; }

        public Uri ApiUrl { get; init; }

        public string Version { get; init; }

        public string Merchantid { get; init; }

        public string Deskey { get; init; }

        public bool Status { get; set; }
    }
}
