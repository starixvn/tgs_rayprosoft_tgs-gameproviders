using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.RedisRepositories.Models;

namespace TGS.GameProviders.RedisRepositories.UnitTests
{
    public class GameProviderScheduleLockRepositoryUnitTest
    {
        [Test]
        public async Task GetAsync()
        {
            var fakeRepository = Substitute.For<IGameProviderScheduleLockRepository>();
            _ = fakeRepository.GetAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(ValueTask.FromResult<GameProviderScheduleLockData>(null));
            var result = await fakeRepository.GetAsync(Arg.Any<string>(), Arg.Any<string>()).ConfigureAwait(false);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task IsLockAsync()
        {
            var fakeRepository = Substitute.For<IGameProviderScheduleLockRepository>();
            _ = fakeRepository.IsLockAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(ValueTask.FromResult<bool>(true));
            var result = await fakeRepository.IsLockAsync(Arg.Any<string>(), Arg.Any<string>()).ConfigureAwait(false);
        }

        [Test]
        public async Task RemoveAsync()
        {
            var fakeRepository = Substitute.For<IGameProviderScheduleLockRepository>();
            _ = fakeRepository.RemoveAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(ValueTask.FromResult<bool>(true));
            var result = await fakeRepository.RemoveAsync(Arg.Any<string>(), Arg.Any<string>()).ConfigureAwait(false);
        }

        [Test]
        public async Task RemoveGameProviderCodeAsync()
        {
            var fakeRepository = Substitute.For<IGameProviderScheduleLockRepository>();
            _ = fakeRepository.RemoveGameProviderCodeAsync(Arg.Any<string>());
            await fakeRepository.RemoveGameProviderCodeAsync(Arg.Any<string>()).ConfigureAwait(false);
        }

        [Test]
        public async Task SetAsync()
        {
            var fakeRepository = Substitute.For<IGameProviderScheduleLockRepository>();
            _ = fakeRepository.SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<GameProviderScheduleLockData>()).Returns(ValueTask.FromResult(true));
            var result = await fakeRepository.SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<GameProviderScheduleLockData>()).ConfigureAwait(false);
        }
    }
}
