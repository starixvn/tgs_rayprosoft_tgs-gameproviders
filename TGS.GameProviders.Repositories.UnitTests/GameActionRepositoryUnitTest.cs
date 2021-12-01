using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameAction;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Repositories.UnitTests
{
    [TestFixture]
    public class GameActionRepositoryUnitTest
    {
        [Test]
        public async Task GetGameActionsByRoundId()
        {
            var roundId = Guid.Parse("317b3829-8390-434a-7375-08d9aef9182c");

            var expected = new GameActionModel[]
            {
                new GameActionModel(
                    Guid.Parse("afa6b677-d93d-4879-7374-08d9aef9182c"),
                    roundId,
                    "4uf7vs2iy8ox",
                    "113995497149",
                    GameActionType.PlaceBet,
                    -10m,
                    0m,
                    0m,
                    0m,
                    null,
                    "113995497149",
                    "4261000_269",
                    null,
                    null,
                    DateTimeOffset.UtcNow.AddMinutes(-1).DateTime,
                    DateTimeOffset.UtcNow.AddMinutes(-1).DateTime,
                    null,
                    null),
                new GameActionModel(
                    Guid.Parse("d9f852fa-280c-4c9f-7377-08d9aef9182c"),
                    roundId,
                    "4uf7vs2iy8ox",
                    "113995497149_settle",
                    GameActionType.LoseBet,
                    0m,
                    10m,
                    10m,
                    -10m,
                    null,
                    "113995497149",
                    "4261000_269",
                    Guid.Parse("afa6b677-d93d-4879-7374-08d9aef9182c"),
                    Guid.Parse("bb2f79ec-28b3-427a-737d-08d9aef9182c"),
                    DateTimeOffset.UtcNow.DateTime,
                    DateTimeOffset.UtcNow.DateTime,
                    null,
                    null)
                };

            var sut = Substitute.For<IGameActionRepository>();
            _ = sut.GetGameActionsByRoundIdAsync(roundId).Returns(expected.ToAsyncEnumerable());

            var result = await sut
                .GetGameActionsByRoundIdAsync(roundId)
                .ToArrayAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(expected.Length, result.Length);
        }
    }
}