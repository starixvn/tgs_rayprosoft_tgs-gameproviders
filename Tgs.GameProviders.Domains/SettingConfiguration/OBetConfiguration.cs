namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record OBetConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
    public string AgentId { get; init; } = default!;
    public decimal MaxBetGrpHdp { get; init; } = default!;
    public decimal MaxBetGrpBasketBall { get; init; } = default!;
    public decimal MaxBetGrpOthersSport { get; init; } = default!;
    public decimal MaxBetGrp1x2 { get; init; } = default!;
    public decimal MaxBetGrpMixParlay { get; init; } = default!;
    public decimal MaxBetGrpOthersOdds { get; init; } = default!;
    public decimal MaxBetGrpSpecial { get; init; } = default!;
}
