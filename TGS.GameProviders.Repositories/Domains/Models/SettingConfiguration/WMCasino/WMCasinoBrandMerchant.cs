namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.WMCasino
{
    public record WMCasinoBrandMerchant
    {
        public WMCasinoAgent[] Agents { get; init; } = default!;
        public string BrandCode { get; init; } = default!;
    }
}