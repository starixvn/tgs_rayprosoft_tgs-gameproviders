using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories.Models.Player
{
    public record PlayerModel(
        Guid ResellerId,
        string ResellerCode,
        Guid LicenseeId,
        string LicenseeCode,
        Guid BrandId,
        string BrandCode,
        string PlayerCode,
        string? IdNumber,
        string Username,
        string? RealName,
        Guid DefaultWalletId,
        string DefaultWalletCurrencyCode,
        PlayerStatus Status
        );
}
