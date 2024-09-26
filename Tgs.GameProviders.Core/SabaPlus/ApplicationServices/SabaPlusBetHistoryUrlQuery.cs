using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public record SabaPlusBetHistoryUrlQuery
{
	[JsonIgnore]
	public string GameProviderCode { get; set; } = default!;

	[Required(ErrorMessage = "Please enter GameId")]
	public int GameId { get; init; } = default!;

	[Required(ErrorMessage = "Please enter ExternalRoundId")]
	public string ExternalRoundId { get; set; }

	[Required(ErrorMessage = "Please enter PlayerCode")]
	public string PlayerCode { get; init; } = default!;

	[DefaultValue(typeof(string), "en-US")]
	public string Language { get; init; } = default!;

	public string ResultId { get; init; } = default!;

	public DateTime SettleDate { get; init; } = default!;
};
