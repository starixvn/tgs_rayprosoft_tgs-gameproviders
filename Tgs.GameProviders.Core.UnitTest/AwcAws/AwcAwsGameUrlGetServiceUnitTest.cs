using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.AwcAws;

[TestFixture]
public class AwcAwsGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<AwcAwsCreatePlayerCommand>>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceAwcServiceGrpc = Substitute.For<IGameInterfaceAwcServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var gameUrl = new Uri("http://www.google.com");

        var configuration = new AwcAwsConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/")
        };

        var configurationData = new AwcConfigurationData(
            AgentId: "thbgpiaelotto",
            Cert: "sJQKnLeKugNHE7yqW7C",
            BetLimit: null);

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
            GameStatus.Active,
            "test",
            null,
            null,
            null,
            Guid.NewGuid(),
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
            GameProviderType.AWC_AWS);

        var query = new AwcAwsGameUrlQuery
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
        var gameProviderCultureCode = "en";
        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_AWS)).Returns(gameModel.GameProviderId);
        _ = fakeSettingRepository.GetAwcAwsConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameModel.GameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetAwcConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameModel.GameProviderId),
            Arg.Is(query.Language)).Returns(gameProviderCultureCode);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameModel);

        var request = new GameInterfaceAwcServiceGameUrlRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configurationData.AgentId,
            Cert = configurationData.Cert,
            PlayerCode = query.PlayerCode,
            BackUrl = query.BackUrl.ToString(),
            IsMobileLogin = query.PlatformType == PlatformType.Mobile,
            ExternalGameId = gameModel.GameExternalId,
            Language = gameProviderCultureCode
        };
        var response = new GameInterfaceAwcServiceGameUrlReply()
        {
            Success = true,
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };
        _ = fakeGameInterfaceAwcServiceGrpc.GameUrlByAwcAwsAsync(request).Returns(response);

        var sut = new AwcAwsGameUrlGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameRepository,
            fakeGameProviderRepository,
            fakeGameInterfaceAwcServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
