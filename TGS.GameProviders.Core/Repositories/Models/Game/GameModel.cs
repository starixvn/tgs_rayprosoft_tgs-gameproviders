using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories.Models.Game
{
    public record GameModel(
         int GameId,
         GameStatus Status,
         string GameExternalId,
         string? Description,
         string? EndpointPath,
         string? RuleUrl,
         Guid GameProviderId,
         GameType GameType,
         GamePlatformType PlatformType,
         bool SubGame,
         bool AvailableToBrand,
         bool HasTurnoverOnTieBet,
         bool Html5,
         string[]? AvailableCurrencies,
         string GameCode,
         string Name,
         string? Localization,
         string? SubGameProviderCode
    );
}
