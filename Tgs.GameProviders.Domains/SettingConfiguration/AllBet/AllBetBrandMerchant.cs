namespace Tgs.GameProviders.Domains.SettingConfiguration.AllBet
{
    public record AllBetBrandMerchant
    {
        public string BrandCode { get; init; } = default!;

        public AllBetAgent[] Agents { get; init; } = default!;
    }
}
