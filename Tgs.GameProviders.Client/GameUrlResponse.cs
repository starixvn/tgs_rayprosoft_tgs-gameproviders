namespace Tgs.GameProviders.Client;

public record GameUrlResponse
{
	/// <summary>
	/// The URL
	/// </summary>
	public Uri? Url { get; set; }
}
