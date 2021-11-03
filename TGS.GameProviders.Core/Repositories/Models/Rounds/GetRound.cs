using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories.Models.Rounds
{
    public record GetRound()
    {
        public Guid Id { get; set; }

        public string ExternalRoundId { get; set; }

        public Guid GameProviderId { get; set; }

        public string PlayerCode { get; set; }

        public int GameId { get; set; }

        public string CurrencyCode { get; set; }

        public RoundStatus Status { get; set; }

        public Guid GameProviderConfigurationId { get; set; }

        public Guid WalletId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public Guid BrandId { get; set; }

        public DateTime? ExternalCreatedOn { get; set; }

        public DateTime? ExternalClosedOn { get; set; }

        public decimal? BetAmount { get; set; }

        public decimal? ValidBetAmount { get; set; }

        public decimal? SettleAmount { get; set; }

        public decimal? Commission { get; set; }

        public decimal? Turnover { get; set; }

        public decimal? WinLoss { get; set; }

        public decimal? TieBetAdjustment { get; set; }

        public ClientRoundStatus ClientStatus { get; set; }

        public decimal? ReopenTurnoverAdjustment { get; set; }

        public DateTime? ReopenedOn { get; set; }

        public decimal? EffectiveBettingPercentage { get; set; }

        public string AdditionalData { get; set; }
    }
}
