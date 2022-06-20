using System.Text.Json;
using ExpectedObjects;
using NSubstitute;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories.UnitTest
{
    [TestClass]
    public class GameProviderRepositoryUnitTest
    {
        [TestMethod]
        public async Task Get_Awc_ConfigurationData()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var gameProviderId = Guid.NewGuid();
            var gameProviderConfigurationId = Guid.NewGuid();
            var awcConfigurationData = new AwcConfigurationData(
                AgentId: "testAgentId",
                Cert: "testCert",
                BetLimit: null);
            var configurationData = JsonSerializer.Serialize(awcConfigurationData);

            _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigModel(
                GameProviderId: gameProviderId,
                GameProviderCode: "AWC_AWS",
                GameProviderConfigId: gameProviderConfigurationId,
                Name: "AWC_AWS-GPI",
                Description: "AWC_AWS-GPI Configuration",
                ClientId: "AWC_AWS-GPI",
                SecretKey: Guid.NewGuid().ToString("N"),
                Status: GameProviderConfigStatus.Active,
                DesktopEndpoint: string.Empty,
                MobileEndpoint: string.Empty,
                ConfigurationData: configurationData,
                IsDefault: false));

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            awcConfigurationData.ToExpectedObject().ShouldEqual(actual);
        }

        [TestMethod]
        public async Task Get_Game_Endpoints()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var desktopEndpoint = "http://desktop.google.com";
            var mobileEndpoint = "http://mobile.google.com";
            var gameProviderId = Guid.NewGuid();
            var gameProviderConfigurationId = Guid.NewGuid();

            _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigModel(
                GameProviderId: gameProviderId,
                GameProviderCode: "AG",
                GameProviderConfigId: gameProviderConfigurationId,
                Name: "AG-PG",
                Description: "AG-PG Configuration",
                ClientId: "AG-PG",
                SecretKey: Guid.NewGuid().ToString("N"),
                Status: GameProviderConfigStatus.Active,
                DesktopEndpoint: desktopEndpoint,
                MobileEndpoint: mobileEndpoint,
                ConfigurationData: string.Empty,
                IsDefault: false));

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetGameEndpointsAsync(gameProviderConfigurationId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(desktopEndpoint, actual.DesktopEndpoint);
            Assert.AreEqual(mobileEndpoint, actual.MobileEndpoint);
        }

        [TestMethod]
        public async Task Get_GameProvider()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var gameProviderId = Guid.NewGuid();

            _ = fakeClient.GameProviderGetAsync(gameProviderId).Returns(new GameProviderModel(
                GameProviderId: gameProviderId,
                Status: GameProviderStatus.Active,
                Description: "KY",
                AutoCloseRound: true,
                SupportTestPlayer: true,
                AllowKickPlayer: true,
                PlaceBetTimeout: 0,
                SettleBetTimeout: 0,
                UniqueId: 1,
                TransferWallet: true,
                GameProviderCode: "KY",
                Name: "KY",
                Localization: null));

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetAsync(gameProviderId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderId, actual.GameProviderId);
        }

        [TestMethod]
        public async Task Get_GameProvider_CultureCode()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var gameProviderId = Guid.NewGuid();
            var cultureCode = "en-US";
            var expected = "en";
            _ = fakeClient.GameProviderGetCultureCodeAsync(gameProviderId, cultureCode).Returns(expected);

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Get_GameProvider_CurrencyCode()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var gameProviderId = Guid.NewGuid();
            var currencyCode = "CNY";
            var expected = "RMB";
            _ = fakeClient.GameProviderGetCurrencyCodeAsync(gameProviderId, currencyCode).Returns(expected);

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Get_GameProvider_Id()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var gameProviderCode = "KY";
            var expected = Guid.NewGuid();
            _ = fakeClient.GameProviderGetGameProviderIdAsync(gameProviderCode).Returns(expected);

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Get_JiliTransfer_ConfigurationData()
        {
            var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

            var gameProviderId = Guid.NewGuid();
            var gameProviderConfigurationId = Guid.NewGuid();
            var awcConfigurationData = new JiliTransferConfigurationData(
                AgentId: "testAgentId");
            var configurationData = JsonSerializer.Serialize(awcConfigurationData);

            _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigModel(
                GameProviderId: gameProviderId,
                GameProviderCode: "JILITF",
                GameProviderConfigId: gameProviderConfigurationId,
                Name: "JILITF-GPI",
                Description: "JILITF-GPI Configuration",
                ClientId: "JILITF-GPI",
                SecretKey: Guid.NewGuid().ToString("N"),
                Status: GameProviderConfigStatus.Active,
                DesktopEndpoint: string.Empty,
                MobileEndpoint: string.Empty,
                ConfigurationData: configurationData,
                IsDefault: false));

            var repository = new GameProviderRepository(fakeClient);

            var actual = await repository.GetJiliTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            awcConfigurationData.ToExpectedObject().ShouldEqual(actual);
        }
    }
}
