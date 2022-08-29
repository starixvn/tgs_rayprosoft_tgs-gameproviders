using System;

namespace Tgs.GameProviders.Core;

public record RoundDetailQuery
{
    public Guid RoundId { get; set; } = default!;

    public string Language { get; init; } = default!;
}
