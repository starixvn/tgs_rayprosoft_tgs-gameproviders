using System;

namespace TGS.GameProviders.Domains.SettingConfiguration.TF
{
    public record TFBrandMerchant
    {
        public Uri LaunchUrl { get; set; } = default!;
        public Uri ApiUrl { get; set; } = default!;
        public string BrandCurrency { get; set; } = default!;
        public string BrandCode { get; set; } = default!;
        public string PublicToken { get; set; } = default!;
        public string PrivateToken { get; set; } = default!;
        public string PartnerId { get; set; } = default!;
    }
}
