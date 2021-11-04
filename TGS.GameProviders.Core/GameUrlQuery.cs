using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core
{
    public record GameUrlQuery
    {
        public GameType GameType { get; init; }

        public int? GameId { get; init; }

        public string BrandCode { get; init; }

        public string PlayerCode { get; init; }

        public string CurrencyCode { get; init; }

        public Guid LoginRecordId { get; init; }

        public PlatformType PlatformType { get; init; }

        public string Language { get; init; }

        public string IPAddress { get; init; }

        public Uri BackUrl { get; init; }
    }
}
