using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SBOService.Protos;
using Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.SBOTransfer;

[TestFixture]
public class SBOTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process_SportsBook()
    {
        var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();

        var query = new SBOTransferGameUrlQuery
        {
            GameType = GameType.Slot,
            GameId = 1,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

        var player = new PlayerModel(
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new SBOTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            CompanyKey = "companyKey",
            Agent = "Agent",
            Currency = "CNY",
            Min = 1,
            Max = 5000,
            MaxPerMatch = 20000,
            CasinoTableLimit = 1
        };
        _ = fakeGameProviderRepository.GetSBOTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var game = new GameModel(
                   1,
                   Guid.NewGuid(),
                   GameProviderType.SBOTransfer,
                   "7_1",
                   GameStatus.Active,
                   null,
                   null,
                   null,
                   GameType.Sportsbook,
                   GamePlatformType.DesktopAndMobile,
                   false,
                   true,
                   false,
                   true,
                   null,
                   "test",
                   "test",
                   null,
                   GameProviderType.SBOTransfer);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(game);

        var gameProviderCultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameProviderId),
             Arg.Is(query.Language)).Returns(gameProviderCultureCode);

        var response = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeGameInterfaceSBOClient.GameUrlBySportsBookAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.CompanyKey),
                Arg.Is(configuration.Agent),
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProviderCultureCode),
                Arg.Is(false)).Returns(response);

        var sut = new SBOTransferGameUrlGetService(
            fakeGameInterfaceSBOClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService,
            fakeGameRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri("http://localhost/"), actual);
    }

    [Test]
    public async Task Normal_Process_VirtualSports()
    {
        var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();

        var query = new SBOTransferGameUrlQuery
        {
            GameType = GameType.Slot,
            GameId = 1,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

        var player = new PlayerModel(
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new SBOTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            CompanyKey = "companyKey",
            Agent = "Agent",
            Currency = "CNY",
            Min = 1,
            Max = 5000,
            MaxPerMatch = 20000,
            CasinoTableLimit = 1
        };
        _ = fakeGameProviderRepository.GetSBOTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var game = new GameModel(
                   1,
                   Guid.NewGuid(),
                   GameProviderType.SBOTransfer,
                   "7_1",
                   GameStatus.Active,
                   null,
                   null,
                   null,
                   GameType.VirtualSportsLobby,
                   GamePlatformType.DesktopAndMobile,
                   false,
                   true,
                   false,
                   true,
                   null,
                   "test",
                   "test",
                   null,
                   GameProviderType.SBOTransfer);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(game);

        var gameProviderCultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameProviderId),
             Arg.Is(query.Language)).Returns(gameProviderCultureCode);

        var response = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeGameInterfaceSBOClient.GameUrlByVirtualSportsAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.CompanyKey),
                Arg.Is(configuration.Agent),
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProviderCultureCode)).Returns(response);

        var sut = new SBOTransferGameUrlGetService(
            fakeGameInterfaceSBOClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService,
            fakeGameRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri("http://localhost/"), actual);
    }
}
