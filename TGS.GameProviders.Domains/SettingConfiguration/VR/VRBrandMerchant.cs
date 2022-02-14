namespace TGS.GameProviders.Domains.SettingConfiguration.VR
{
    public record VRBrandMerchant
    {
        public string BrandCode { get; init; } = default!;

        public Uri ApiUrl { get; init; } = default!;

        public string Version { get; init; } = default!;

        public string Merchantid { get; init; } = default!;

        public string Deskey { get; init; } = default!;

        public bool Status { get; set; }
    }
}
