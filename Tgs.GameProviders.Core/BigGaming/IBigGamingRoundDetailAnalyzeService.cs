using Tgs.GameProviders.Core.BigGaming.Models;

namespace Tgs.GameProviders.Core.BigGaming
{
    public interface IBigGamingRoundDetailAnalyzeService
    {
        string GetAnalyze(BigGamingVideoRoundQueryItemResponse source);
    }
}
