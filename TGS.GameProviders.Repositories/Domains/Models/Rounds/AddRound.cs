using System;

namespace TGS.GameProviders.Repositories.Domains.Models.Rounds
{
    public class AddRound
    {
        public string AdditionalData { get; set; }

        public decimal? BetAmount { get; init; }

        public Guid BrandId { get; init; }

        public ClientRoundStatus ClientStatus { get; init; }

        public DateTimeOffset? ClosedOn { get; init; }

        public decimal? Commission { get; init; }

        public DateTimeOffset CreatedOn { get; init; }

        public string CurrencyCode { get; set; }

        public DateTimeOffset? ExternalClosedOn { get; init; }

        public DateTimeOffset? ExternalCreatedOn { get; init; }

        public string ExternalRoundId { get; set; }

        public int GameId { get; init; }

        public Guid GameProviderConfigurationId { get; init; }

        public Guid GameProviderId { get; init; }

        public string PlayerCode { get; init; }

        public decimal? SettleAmount { get; init; }

        public RoundStatus Status { get; init; }

        public decimal? TieBetAdjustment { get; init; }

        public decimal? Turnover { get; init; }

        public DateTimeOffset UpdatedOn { get; init; }

        public decimal? ValidBetAmount { get; init; }

        public Guid WalletId { get; init; }

        public decimal? WinLoss { get; init; }
    }
}
