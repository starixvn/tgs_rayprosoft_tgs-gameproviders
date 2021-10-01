using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;

namespace TGS.GameProviders.Repositories.UnitTests
{
    [TestFixture]
    public class RoundRepositoryUnitTest
    {
        [Test]
        public async Task AddRoundAsync()
        {
            var data = new AddRound
            {
                ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
                GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                PlayerCode = "4ues8xikza7o",
                GameId = 500001,
                CurrencyCode = "CNY",
                Status = RoundStatus.Open,
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                CreatedOn = DateTimeOffset.UtcNow,
                ClosedOn = null,
                UpdatedOn = DateTimeOffset.UtcNow,
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTimeOffset.UtcNow,
                ExternalClosedOn = null,
                BetAmount = 10.000000m,
                ValidBetAmount = 10.000000m,
                SettleAmount = null,
                Commission = 0,
                Turnover = 10.000000m,
                WinLoss = null,
                TieBetAdjustment = null,
                ClientStatus = ClientRoundStatus.Open,
                AdditionalData = "{}"
            };

            var assert = 1;
            var fakeRoundRepository = Substitute.For<IRoundRepository>();
            _ = fakeRoundRepository.AddRoundAsync(data).Returns(Task.FromResult(assert));
            await fakeRoundRepository.AddRoundAsync(data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetRound()
        {
            var data = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");

            var assert = new GetRound()
            {
                Id = data,
                ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
                GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                PlayerCode = "4ues8xikza7o",
                GameId = 500001,
                CurrencyCode = "CNY",
                Status = RoundStatus.Open,
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                CreatedOn = DateTimeOffset.UtcNow.DateTime,
                ClosedOn = null,
                UpdatedOn = DateTimeOffset.UtcNow.DateTime,
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTimeOffset.UtcNow.DateTime,
                ExternalClosedOn = null,
                BetAmount = 10.000000m,
                ValidBetAmount = 10.000000m,
                SettleAmount = null,
                Commission = 0,
                Turnover = 10.000000m,
                WinLoss = null,
                TieBetAdjustment = null,
                ClientStatus = ClientRoundStatus.Open
            };
            var fakeRoundRepository = Substitute.For<IRoundRepository>();
            _ = fakeRoundRepository.GetRoundAsync(data).Returns(Task.FromResult(assert));
            var result = await fakeRoundRepository.GetRoundAsync(data).ConfigureAwait(false);
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

            var assert = new GetRound()
            {
                Id = Guid.NewGuid(),
                ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
                GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                PlayerCode = "4ues8xikza7o",
                GameId = 500001,
                CurrencyCode = "CNY",
                Status = RoundStatus.Open,
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                CreatedOn = DateTimeOffset.UtcNow.DateTime,
                ClosedOn = null,
                UpdatedOn = DateTimeOffset.UtcNow.DateTime,
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTimeOffset.UtcNow.DateTime,
                ExternalClosedOn = null,
                BetAmount = 10.000000m,
                ValidBetAmount = 10.000000m,
                SettleAmount = null,
                Commission = 0,
                Turnover = 10.000000m,
                WinLoss = null,
                TieBetAdjustment = null,
                ClientStatus = ClientRoundStatus.Open
            };
            var fakeRoundRepository = Substitute.For<IRoundRepository>();
            _ = fakeRoundRepository.GetRoundAsync(data.GameProviderId, data.ExternalRoundId, data.GameId).Returns(Task.FromResult(assert));
            var result = await fakeRoundRepository.GetRoundAsync(data.GameProviderId, data.ExternalRoundId, data.GameId).ConfigureAwait(false);
            Assert.AreEqual(assert, result);
        }
    }
}
