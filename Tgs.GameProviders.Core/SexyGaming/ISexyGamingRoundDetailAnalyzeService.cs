using Tgs.GameProviders.Core.SexyGaming.Models;

namespace Tgs.GameProviders.Core.SexyGaming;

public interface ISexyGamingRoundDetailAnalyzeService
{
    string GetAnalyze(string externalGameId, SexyGamingGameRecord source);
}
