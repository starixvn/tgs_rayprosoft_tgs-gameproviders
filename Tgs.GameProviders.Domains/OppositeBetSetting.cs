namespace Tgs.GameProviders.Domains;

public record OppositeBetSetting
{
    // game external ids
    public string[] Games { get; init; } = default!;

    public Pattern[] Patterns { get; init; } = default!;

    public record Pattern(string BetType, int GroupId);
}
