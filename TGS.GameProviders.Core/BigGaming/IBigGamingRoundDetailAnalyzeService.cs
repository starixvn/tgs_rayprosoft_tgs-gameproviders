using TGS.GameProviders.Core.BigGaming.Models;

namespace TGS.GameProviders.Core.BigGaming
{
    public interface IBigGamingRoundDetailAnalyzeService
    {
        string GetAnalyze(BigGamingVideoRoundQueryItemResponse source);
    }
}
