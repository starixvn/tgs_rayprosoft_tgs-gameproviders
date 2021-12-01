using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Repositories.UnitTests
{
    [TestFixture]
    public class RoundRepositoryUnitTest
    {
        [Test]
        public async Task GetRound()
        {
            var data = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");

            var assert = new RoundModel(
                 data,
                 "cc4d6693-417d-401a-8ccf-68f25382a134",
                 new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                 "4ues8xikza7o",
                 500001,
                 "CNY",
                 RoundStatus.Open,
                 new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                 new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                 DateTimeOffset.UtcNow.DateTime,
                 null,
                 DateTimeOffset.UtcNow.DateTime,
                 new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                 DateTimeOffset.UtcNow.DateTime,
                 null,
                 10.000000m,
                 10.000000m,
                 null,
                 0,
                 10.000000m,
                 null,
                 null,
                 ClientRoundStatus.Open,
                 null,
                 null,
                 null,
                 String.Empty
                );
            var fakeRoundRepository = Substitute.For<IRoundRepository>();
            _ = fakeRoundRepository.GetAsync(data).Returns(Task.FromResult(assert));
            var result = await fakeRoundRepository.GetAsync(data).ConfigureAwait(false);
            Assert.AreEqual(assert, result);
        }

        [Test]
        public async Task GetRoundByExternalRoundId()
        {
            var data = new
            {
                GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
                GameId = 500001
            };

            var assert = new RoundModel(
                 Guid.NewGuid(),
                 "cc4d6693-417d-401a-8ccf-68f25382a134",
                 new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                 "4ues8xikza7o",
                 500001,
                 "CNY",
                 RoundStatus.Open,
                 new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                 new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                 DateTimeOffset.UtcNow.DateTime,
                 null,
                 DateTimeOffset.UtcNow.DateTime,
                 new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                 DateTimeOffset.UtcNow.DateTime,
                 null,
                 10.000000m,
                 10.000000m,
                 null,
                 0,
                 10.000000m,
                 null,
                 null,
                 ClientRoundStatus.Open,
                 null,
                 null,
                 null,
                 String.Empty
                );

            var fakeRoundRepository = Substitute.For<IRoundRepository>();
            _ = fakeRoundRepository.GetAsync(data.GameProviderId, data.ExternalRoundId, data.GameId).Returns(Task.FromResult(assert));
            var result = await fakeRoundRepository.GetAsync(data.GameProviderId, data.ExternalRoundId, data.GameId).ConfigureAwait(false);
            Assert.AreEqual(assert, result);
        }
    }
}
