namespace TGS.GameProviders.Domains.SettingConfiguration.Avia
{
    public class AviaBrandMerchant
    {
        public Uri ApiUrl { get; init; } = default!;

        public string Authorization { get; set; } = default!;

        public string BrandCode { get; init; } = default!;

        public string PlayerPassword { get; init; } = default!;

        public bool Status { get; init; }
    }
}
