using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Core.AwcLudo.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcLudo;

[TestFixture]
public class AwcLudoPlayerFundCheckingGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceAwcServiceGrpc = Substitute.For<IGameInterfaceAwcServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new AwcLudoConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/")
        };

        var configurationData = new AwcConfigurationData(
            AgentId: "testAgentId",
            Cert: "testCert",
            BetLimit: null);

        var playerModel = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");

        var query = new AwcLudoPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_LUDO)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetAwcLudoConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetAwcConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);
        _ = fakeGameInterfaceAwcServiceGrpc.FundOrderCheckingAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configurationData.AgentId),
            Arg.Is(configurationData.Cert),
            Arg.Is(query.OrderId)).Returns(true);

        var sut = new AwcLudoPlayerFundCheckingGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceAwcServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(true, actual.OrderStatus);
    }
}
