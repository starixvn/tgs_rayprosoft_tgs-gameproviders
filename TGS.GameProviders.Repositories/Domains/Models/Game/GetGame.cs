using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Repositories.Domains.Models.Game
{
    public record GetGame
    {
        public int GameId { get; set; }
        public short Status { get; set; }
        public string ExternalId { get; set; } = default!;
        public string? Description { get; set; }
        public string? EndpointPath { get; set; }
        public string? RuleUrl { get; set; }
        public Guid GameProviderId { get; set; }
        public GameType Type { get; set; }
        public short PlatformType { get; set; }
        public bool SubGame { get; set; }
        public bool AvailableToBrand { get; set; }
        public bool HasTurnoverOnTieBet { get; set; }
        public bool Html5 { get; set; }
        public string[]? AvailableCurrencies { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; } = default!;
        public string UpdatedByUsername { get; set; } = default!;
        public string? Localization { get; set; }
        public string? SubGameProviderCode { get; set; }
    }
}
