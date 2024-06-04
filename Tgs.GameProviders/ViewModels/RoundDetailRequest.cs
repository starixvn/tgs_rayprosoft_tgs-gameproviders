namespace Tgs.GameProviders.ViewModels;

public record RoundDetailRequest
{
	public string Language { get; init; } = default!;
	public DateTimeOffset ExternalCreatedOn { get; init; }
}
