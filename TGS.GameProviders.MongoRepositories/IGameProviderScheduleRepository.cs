using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TGS.GameProviders.MongoRepositories.Models;

namespace TGS.GameProviders.MongoRepositories
{
    public interface IGameProviderScheduleRepository
    {
        ValueTask<GameProviderSchedule> GetAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        ValueTask<UpdateResult> MergeAsync(GameProviderSchedule data);
    }
}
