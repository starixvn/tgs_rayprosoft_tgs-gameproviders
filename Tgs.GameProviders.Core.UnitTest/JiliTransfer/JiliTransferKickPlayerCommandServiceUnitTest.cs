using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.JiliTransfer;

[TestFixture]
public class JiliTransferKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
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

        var command = new JiliTransferKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.JiliTransfer)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetJiliTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(command.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetJiliTransferConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);

        var sut = new JiliTransferKickPlayerCommandService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceJiliServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceJiliServiceGrpc
            .Received(1)
            .KickPlayerAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configurationData.AgentId),
            Arg.Is(configuration.AgentKey),
            Arg.Is(command.PlayerCode));
    }
}
