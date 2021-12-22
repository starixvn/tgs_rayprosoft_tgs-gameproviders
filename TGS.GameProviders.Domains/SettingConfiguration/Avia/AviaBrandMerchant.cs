using System;

namespace TGS.GameProviders.Domains.SettingConfiguration.Avia
{
    public class AviaBrandMerchant
    {
        public Uri ApiUrl { get; init; }

        public string Authorization { get; set; }

        public string BrandCode { get; init; }

        public string PlayerPassword { get; init; }

        public bool Status { get; init; }
    }
}
