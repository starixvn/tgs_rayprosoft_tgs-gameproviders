using System;
using Tgs.GameProviders.Core.Evolution.Models;

namespace Tgs.GameProviders.Core.Evolution;

public interface IEvolutionRoundDetailAnalyzeService
{
    string GetAnalyze(EvolutionGamHistoryRoundResponse source);

    string GetAnalyzeSlot(Uri apiUrl, EvolutionGamHistoryRoundResponse source);
}
