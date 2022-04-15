namespace Tgs.GameProviders.Domains.SettingConfiguration.WMCasino
{
    public record WMCasinoBrandMerchant
    {
        public WMCasinoAgent[] Agents { get; init; } = default!;
        public string BrandCode { get; init; } = default!;
    }
}
