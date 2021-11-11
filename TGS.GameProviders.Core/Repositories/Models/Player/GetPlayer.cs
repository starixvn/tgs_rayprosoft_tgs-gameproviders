using System;

namespace TGS.GameProviders.Core.Repositories.Models.Player
{
    public record GetPlayer
    {
        public Guid LicenseeId { get; set; }
        public Guid BrandId { get; set; }
        public string BrandCode { get; set; }
        public string PlayerCode { get; set; } = default!;
        public string? IdNumber { get; set; }
        public string Username { get; set; } = default!;
        public string? RealName { get; set; }
        public Guid DefaultWalletId { get; set; }
        public string DefaultWalletCurrencyCode { get; set; } = default!;
        public DateTime RegisteredOn { get; set; }
        public DateTime LastActiveOn { get; set; }
        public short Status { get; set; }
        public int BetLimitId { get; set; }
        public short Type { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Qq { get; set; }
        public string? Wechat { get; set; }
        public Guid? AgentId { get; set; }
        public bool IdentityAuthentication { get; set; }
        public bool PhoneAuthentication { get; set; }
        public decimal AccumulatedDeposit { get; set; }
        public Guid? VipId { get; set; }
        public string? UpdatedByUsername { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? FirstDepositOn { get; set; }
        public string? AreaCode { get; set; }
    }
}
