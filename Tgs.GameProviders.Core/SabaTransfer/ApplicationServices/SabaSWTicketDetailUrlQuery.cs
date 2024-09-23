namespace Tgs.GameProviders.Core.SabaSW.ApplicationServices;

public record SabaSWTicketDetailUrlQuery
{
	public string PlayerCode { get; init; } = default!;

	public long RefId { get; set; }

	public string Language { get; init; } = default!;

	public long? BaseCurrency { get; set; }

	public string PlayerUsername { get; set; }
}
