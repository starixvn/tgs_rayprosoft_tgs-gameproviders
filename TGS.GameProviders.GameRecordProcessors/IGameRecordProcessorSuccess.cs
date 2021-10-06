using System.Threading.Tasks;

namespace TGS.GameProviders.GameRecordProcessors
{
    public interface IGameRecordProcessorSuccess
    {
        Task ProcessorAsync(GameRoundsData data);
    }
}
