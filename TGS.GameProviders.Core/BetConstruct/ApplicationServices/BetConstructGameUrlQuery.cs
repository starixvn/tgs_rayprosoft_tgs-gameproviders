using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.BetConstruct.ApplicationServices
{
    public record BetConstructGameUrlQuery
    {
        public GameType GameType { get; init; }

        public int? GameId { get; init; }

        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public Guid LoginRecordId { get; init; }

        public PlatformType PlatformType { get; init; }

        public string Language { get; init; } = default!;

        public string IPAddress { get; init; } = default!;

        public Uri BackUrl { get; init; } = default!;
        public string LobbyDomain { get; init; }
    }
}