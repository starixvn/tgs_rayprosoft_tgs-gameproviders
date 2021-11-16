using TGS.GameProviders.Core.SexyGaming.Models;

namespace TGS.GameProviders.Core.SexyGaming
{
    public interface ISexyGamingRoundDetailAnalyzeService
    {
        string GetAnalyze(string externalGameId, SexyGamingGameRecord source);
    }
}
