using System.Threading.Tasks;

namespace TGS.GameProviders.Core.GameRecordProcessor
{
    public interface IGameRecordProcessorFailed
    {
        Task ProcessorAsync(GameRoundsData data);
    }
}
