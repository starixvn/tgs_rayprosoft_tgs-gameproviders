using System.Threading.Tasks;
using TGS.GameProviders.GameRecord.Core.Processors;

namespace TGS.GameProviders.GameRecord.Core
{
    public interface IGameRecordProcessorSuccess
    {
        Task ProcessorAsync(GameRoundsData data);
    }
}
