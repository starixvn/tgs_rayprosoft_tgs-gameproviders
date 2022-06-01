namespace Tgs.GameProviders.Domains.SettingConfiguration.Joker;

public record JokerBrandMerchant
{
    public string BrandCode { get; init; } = default!;

    public IEnumerable<JokerAgent> Agents { get; init; } = default!;
}
