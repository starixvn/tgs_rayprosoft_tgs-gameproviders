namespace Tgs.GameProviders.Client;
public record TicketDetailUrlRequest()
{
	public required string GameProviderCode { get; set; }

	public required string PlayerCode { get; set; }

	public required string ExternalRoundId { get; set; }

	public int GameId { get; init; }

	public string Language { get; set; } = "en-US";

	public string? BaseCurrency { get; set; }

	public string? PlayerUsername { get; set; }
}
