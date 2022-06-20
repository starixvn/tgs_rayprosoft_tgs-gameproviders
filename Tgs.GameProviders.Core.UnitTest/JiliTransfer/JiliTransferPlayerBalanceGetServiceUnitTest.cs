using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameInterface.JiliService.Protos;
using Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.JiliTransfer;

[TestFixture]
public class JiliTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<JiliTransferCreatePlayerCommand>>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceJiliServiceGrpc = Substitute.For<IGameInterfaceJiliServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new JiliTransferConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            AgentKey = Guid.NewGuid().ToString("N")
        };

        var configurationData = new JiliTransferConfigurationData(
            AgentId: "testAgentId");

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);

        var query = new JiliTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.JiliTransfer)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetJiliTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetJiliTransferConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);

        var response = new GameInterfaceJiliServiceBalanceReply()
        {
            Success = true,
            IsPlayerExist = true,
            Balance = 100m
        };
        _ = fakeGameInterfaceJiliServiceGrpc.GetBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configurationData.AgentId),
            agentKey: Arg.Is(configuration.AgentKey),
            playerCode: Arg.Is(query.PlayerCode)).Returns(response);

        var sut = new JiliTransferPlayerBalanceGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameProviderRepository,
            fakeGameInterfaceJiliServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.Balance.ToDecimal(), actual.Value);
    }
}
