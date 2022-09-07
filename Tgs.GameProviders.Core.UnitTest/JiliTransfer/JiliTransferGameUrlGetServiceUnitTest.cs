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
public class JiliTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<JiliTransferCreatePlayerCommand>>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceJiliServiceGrpc = Substitute.For<IGameInterfaceJiliServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var gameUrl = new Uri("http://www.google.com");

        var configuration = new JiliTransferConfiguration(
            ApiUrl: new("https://tttint.onlinegames22.com/"),
            AgentKey: Guid.NewGuid().ToString("N"));

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

        var gameModel = new GameModel(
            1,
            Guid.NewGuid(),
            GameProviderType.JiliTransfer,
            "test",
            GameStatus.Active,
            null,
            null,
            null,
            GameType.Slot,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "test",
            "test",
            null,
            GameProviderType.JiliTransfer);

        var query = new JiliTransferGameUrlQuery
        {
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };
        var gameProviderCultureCode = "en-US";
        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.JiliTransfer)).Returns(gameModel.GameProviderId);
        _ = fakeSettingRepository.GetJiliTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameModel.GameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetJiliTransferConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameModel.GameProviderId),
            Arg.Is(query.Language)).Returns(gameProviderCultureCode);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameModel);

        var response = new GameInterfaceJiliServiceGameUrlReply()
        {
            Success = true,
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };
        _ = fakeGameInterfaceJiliServiceGrpc.GetGameUrlAsync(
             apiUrl: Arg.Is(configuration.ApiUrl),
             agentId: Arg.Is(configurationData.AgentId),
             agentKey: Arg.Is(configuration.AgentKey),
             playerCode: Arg.Is(playerModel.PlayerCode),
             externalGameId: Arg.Is(gameModel.GameExternalId),
             language: Arg.Is(gameProviderCultureCode)
            ).Returns(response);

        var sut = new JiliTransferGameUrlGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameRepository,
            fakeGameProviderRepository,
            fakeGameInterfaceJiliServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
