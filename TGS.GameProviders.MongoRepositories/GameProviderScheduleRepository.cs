using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.MongoRepositories.Options;

namespace TGS.GameProviders.MongoRepositories
{
    internal class GameProviderScheduleRepository : IGameProviderScheduleRepository
    {
        private readonly IMongoCollection<GameProviderSchedule> _collection;
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _mongoClient;
        private readonly MongoDBOption _mongoDBOption;

        public GameProviderScheduleRepository(IOptions<MongoDBOption> mongoDBOption)
        {
            _mongoDBOption = mongoDBOption.Value;
            var builder = new MongoUrlBuilder(_mongoDBOption.GameProvider);
            _mongoClient = new MongoClient(_mongoDBOption.GameProvider);
            _database = _mongoClient.GetDatabase(builder.DatabaseName);
            _collection = _database.GetCollection<GameProviderSchedule>(typeof(GameProviderSchedule).Name);
        }

        public async ValueTask<GameProviderSchedule> GetAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode)
        {
            var filter = Builders<GameProviderSchedule>.Filter.And(
                Builders<GameProviderSchedule>.Filter.Where(p => p.GameProviderId == gameProviderId),
                Builders<GameProviderSchedule>.Filter.Where(p => p.BrandCode == brandCode),
                Builders<GameProviderSchedule>.Filter.Where(p => p.AgentId == agentId),
                Builders<GameProviderSchedule>.Filter.Where(p => p.CurrencyCode.Contains(currencyCode))
            );

            var result = await _collection
                .FindAsync<GameProviderSchedule>(filter)
                .ConfigureAwait(false);

            return await result.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async ValueTask<UpdateResult> MergeAsync(GameProviderSchedule data)
        {
            var filter = Builders<GameProviderSchedule>.Filter
                .Eq(x => x.Id, data.Id);

            var update = Builders<GameProviderSchedule>.Update
                .Set(s => s.GameProviderId, data.GameProviderId)
                .Set(s => s.GameProviderCode, data.GameProviderCode)
                .Set(s => s.BrandCode, data.BrandCode)
                .Set(s => s.AgentId, data.AgentId)
                .Set(s => s.CurrencyCode, data.CurrencyCode)
                .Set(s => s.Config, data.Config)
                .Set(s => s.CreatedOn, data.CreatedOn)
                .Set(s => s.UpdatedOn, data.UpdatedOn);

            var option = new UpdateOptions()
            {
                IsUpsert = true
            };

            var result = await _collection
                .UpdateOneAsync(filter, update, option)
                .ConfigureAwait(false);

            return result;
        }
    }
}
