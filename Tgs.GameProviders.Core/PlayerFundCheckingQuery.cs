using System;

namespace Tgs.GameProviders.Core;

public record PlayerFundCheckingQuery
{
	public string PlayerCode { get; init; }
	public string OrderId { get; set; }
	public DateTimeOffset OrderOn { get; set; }
}
