using System.Threading.Tasks;

namespace TGS.GameProviders.Core.GameRecordProcessor
{
    public interface IGameRecordProcessorSuccess
    {
        Task ProcessorAsync(GameRoundsData data);
    }
}
