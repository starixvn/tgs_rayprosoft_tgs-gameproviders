using System;
using System.Collections.Generic;
using TGS.GameProviders.Repositories.Domains;

namespace TGS.GameProviders.ApiClients.SBO
{
    public static class SBOUtility
    {
        private static readonly IReadOnlyDictionary<GameType, string> _Portfolios = new Dictionary<GameType, string>
        {
            [GameType.Sportsbook] = "SportsBook",
            [GameType.VirtualSports] = "VirtualSports",
            [GameType.VirtualSportsLobby] = "VirtualSports"
        };

        public static string GetPortfolio(GameType gameType) =>
            !_Portfolios.TryGetValue(gameType, out var portfolio)
                ? throw new InvalidOperationException($"No portfolio mapping is found: {gameType}")
                : portfolio;
    }
}