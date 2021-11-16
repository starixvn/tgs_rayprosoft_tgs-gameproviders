using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Evolution.Models;

namespace TGS.GameProviders.Core.Evolution
{
    public interface IEvolutionRoundDetailAnalyzeService
    {
        string GetAnalyze(EvolutionGamHistoryRoundResponse source);
        string GetAnalyzeSlot(Uri apiUrl, EvolutionGamHistoryRoundResponse source);
    }
}
