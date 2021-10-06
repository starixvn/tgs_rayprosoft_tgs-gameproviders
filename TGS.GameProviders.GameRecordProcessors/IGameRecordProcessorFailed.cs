using System.Threading.Tasks;

namespace TGS.GameProviders.GameRecordProcessors
{
    public interface IGameRecordProcessorFailed
    {
        Task ProcessorAsync(GameRoundsData data);
    }
}
