using System;
using TGS.GameProviders.Core.Evolution.Models;

namespace TGS.GameProviders.Core.Evolution
{
    public interface IEvolutionRoundDetailAnalyzeService
    {
        string GetAnalyze(EvolutionGamHistoryRoundResponse source);

        string GetAnalyzeSlot(Uri apiUrl, EvolutionGamHistoryRoundResponse source);
    }
}
