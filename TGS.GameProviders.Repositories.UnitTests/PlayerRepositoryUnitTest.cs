using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.UnitTests
{
    [TestFixture]
    public class PlayerRepositoryUnitTest
    {
        [Test]
        public async Task GetPlayerAsync()
        {
            var fakeRoundRepository = Substitute.For<IPlayerRepository>();
            _ = fakeRoundRepository.GetPlayerAsync(Arg.Any<string>()).Returns(Task.FromResult<GetPlayer>(null));
            var result = await fakeRoundRepository.GetPlayerAsync(Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task GetPlayerByUsernameAsync()
        {
            var fakeRoundRepository = Substitute.For<IPlayerRepository>();
            _ = fakeRoundRepository.GetPlayerAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult<GetPlayer>(null));
            var result = await fakeRoundRepository.GetPlayerAsync(Arg.Any<Guid>(), Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }
    }
}
