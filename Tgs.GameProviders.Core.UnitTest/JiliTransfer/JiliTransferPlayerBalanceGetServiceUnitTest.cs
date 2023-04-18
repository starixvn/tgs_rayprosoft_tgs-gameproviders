using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.JiliTransfer;

[TestFixture]
public class JiliTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<JiliTransferCreatePlayerCommand>>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new JiliTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
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
            .GetJiliTransferConfiguration(
                Arg.Is(gameProviderConfigurationId))
            .Returns(configuration);

        var response = new BalanceReply()
        {
            Success = true,
            IsPlayerExist = true,
            Balance = 100m
        };
        _ = fakeClient.GetBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            agentKey: Arg.Is(configuration.AgentKey),
            playerCode: Arg.Is(query.PlayerCode)).Returns(response);

        var sut = new JiliTransferPlayerBalanceGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameProviderRepository,
            fakeClient,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.Balance.ToDecimal(), actual.Value);
    }
}
