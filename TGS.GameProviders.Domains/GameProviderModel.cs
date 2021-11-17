using System;

namespace TGS.GameProviders.Domains
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
