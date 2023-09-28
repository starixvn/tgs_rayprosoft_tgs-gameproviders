using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.JiliTransfer;

public class JiliTransferPlayerFundCheckingGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new JiliTransferPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                Arg.Is(GameProviderType.JiliTransfer))
            .Returns(gameProviderId);

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
        _ = fakePlayerRepository
        .GetAsync(
                Arg.Is(query.PlayerCode))
            .Returns(playerModel);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService
            .GetBrandGameProviderConfigurationIdAsync(
                Arg.Is(gameProviderId),
                Arg.Is(playerModel.BrandId))
            .Returns(gameProviderConfigurationId);

        var configuration = new JiliTransferConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            AgentKey = Guid.NewGuid().ToString("N"),
            AgentId = "testAgentId"
        };
        _ = fakeGameProviderRepository
            .GetJiliTransferConfigurationAsync(
                Arg.Is(gameProviderConfigurationId))
            .Returns(configuration);

        _ = fakeClient.FundOrderCheckingAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            agentKey: Arg.Is(configuration.AgentKey),
            transactionId: Arg.Is(query.OrderId)).Returns(true);

        var sut = new JiliTransferPlayerFundCheckingGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeClient,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query);

        Assert.True(actual);
    }
}
