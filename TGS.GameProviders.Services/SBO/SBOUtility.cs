using System.Collections.Generic;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core.SBO;

namespace TGS.GameProviders.Services.SBO
{
    internal class SBOUtility : ISBOUtility
    {
        private static readonly IReadOnlyDictionary<GameType, string> _Portfolios = new Dictionary<GameType, string>
        {
            [GameType.Sportsbook] = "SportsBook",
            [GameType.VirtualSports] = "VirtualSports",
            [GameType.VirtualSportsLobby] = "VirtualSports"
        };

        public string GetAgentId(
            string agentPrefix,
            string brandCode,
            string currencyCode)
            => $"{agentPrefix}{brandCode.ToUpper()}{currencyCode.Replace("_", string.Empty).ToUpper()}";

        public string GetPortfolio(
                    GameType gameType)
            => !_Portfolios.TryGetValue(gameType, out var portfolio)
                ? null
                : portfolio;
    }
}
