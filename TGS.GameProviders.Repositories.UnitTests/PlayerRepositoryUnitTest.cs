using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Player;

namespace TGS.GameProviders.Repositories.UnitTests
{
    [TestFixture]
    public class PlayerRepositoryUnitTest
    {
        [Test]
        public async Task GetPlayerAsync()
        {
            var fakeRoundRepository = Substitute.For<IPlayerRepository>();
            _ = fakeRoundRepository.GetAsync(Arg.Any<string>()).Returns(Task.FromResult<PlayerModel>(null));
            var result = await fakeRoundRepository.GetAsync(Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task GetPlayerByUsernameAsync()
        {
            var fakeRoundRepository = Substitute.For<IPlayerRepository>();
            _ = fakeRoundRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult<PlayerModel>(null));
            var result = await fakeRoundRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }
    }
}
