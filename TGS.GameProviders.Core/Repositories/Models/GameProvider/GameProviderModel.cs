using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories.Models.GameProvider
{
    public record GameProviderModel(
        Guid GameProviderId,
        GameProviderStatus Status,
        string? Description,
        bool AutoCloseRound,
        bool SupportTestPlayer,
        bool AllowKickPlayer,
        short PlaceBetTimeout,
        short SettleBetTimeout,
        int UniqueId,
        bool TransferWallet,
        string GameProviderCode,
        string Name,
        string? Localization
        );
}
