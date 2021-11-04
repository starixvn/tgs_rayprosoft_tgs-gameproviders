using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;

namespace TGS.GameProviders.Repositories.UnitTests
{
    [TestFixture]
    public class GameRepositoryUnitTest
    {
        [Test]
        public async Task GetGameAsync()
        {
            var fakeRoundRepository = Substitute.For<IGameRepository>();
            _ = fakeRoundRepository.GetGameAsync(Arg.Any<int>()).Returns(Task.FromResult<GetGame>(null));
            var result = await fakeRoundRepository.GetGameAsync(Arg.Any<int>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task GetGameByExternalGameIdAsync()
        {
            var fakeRoundRepository = Substitute.For<IGameRepository>();
            _ = fakeRoundRepository.GetGameAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult<GetGame>(null));
            var result = await fakeRoundRepository.GetGameAsync(Arg.Any<Guid>(), Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }
    }
}
