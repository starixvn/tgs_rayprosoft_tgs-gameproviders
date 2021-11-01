using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.MongoRepositories.Models;

namespace TGS.GameProviders.Core
{
    public interface ILYGameProviderScheduleService
    {

        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime);

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        LYGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);
    }
}
