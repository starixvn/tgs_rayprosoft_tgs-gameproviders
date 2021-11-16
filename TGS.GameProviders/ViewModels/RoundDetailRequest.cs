using System;

namespace TGS.GameProviders.ViewModels
{
    public record RoundDetailRequest
    {
        public Guid RoundId { get; set; } = default!;

        public string Language { get; init; } = default!;
    }
}
