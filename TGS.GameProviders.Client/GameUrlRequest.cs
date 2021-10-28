using System;
using TGS.GameProviders.Repositories.Domains;

namespace TGS.GameProviders.Client
{
    public record GameUrlRequest
    {
        public GameType GameType { get; init; } = default!;

        public int GameId { get; init; } = default!;

        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public Guid LoginRecordId { get; init; } = default!;

        public PlatformType PlatformType { get; init; } = PlatformType.Desktop;

        public string Language { get; init; } = default!;

        public string IPAddress { get; init; } = default!;

        public Uri? BackUrl { get; init; }
    }
}
