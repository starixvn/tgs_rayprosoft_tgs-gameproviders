using System;

namespace Tgs.GameProviders.Core;

public record RoundDetailQuery
{
	public Guid GameBetId { get; set; } = default!;

	public string Language { get; init; } = default!;
}
