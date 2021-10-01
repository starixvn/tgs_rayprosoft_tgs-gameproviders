using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.MongoRepositories.Models;

namespace TGS.GameProviders.MongoRepositories.UnitTests
{
    public class GameProviderScheduleRepositoryUnitTest
    {
        [Test]
        public async Task GetGameAsync()
        {
            var fakeRoundRepository = Substitute.For<IGameProviderScheduleRepository>();
            _ = fakeRoundRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(ValueTask.FromResult<GameProviderSchedule>(null));
            var result = await fakeRoundRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task MergeAsync()
        {
            var fakeRoundRepository = Substitute.For<IGameProviderScheduleRepository>();
            _ = fakeRoundRepository.MergeAsync(Arg.Any<GameProviderSchedule>());
            await fakeRoundRepository.MergeAsync(Arg.Any<GameProviderSchedule>()).ConfigureAwait(false);
        }
    }
}
