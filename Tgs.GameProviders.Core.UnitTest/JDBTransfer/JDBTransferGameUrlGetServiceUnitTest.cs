using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.JDBService.Protos;
using Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.JDBTransfer;

[TestFixture]
public class JDBTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceJDBClient = Substitute.For<IGameInterfaceJDBClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();

        var query = new JDBTransferGameUrlQuery
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
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.JDBTransfer)).Returns(gameProviderId);

        var player = new PlayerInfo(
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new JDBTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            Domain = "Domain",
            Agent = "Agent",
            AesIV = "AesIV",
            AesKey = "AesKey"
        };
        _ = fakeGameProviderRepository.GetJDBTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.JDBTransfer,
            GameExternalId: "7_1",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "test",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.JDBTransfer);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameInfo);

        var gameProviderCultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameProviderId),
             Arg.Is(query.Language)).Returns(gameProviderCultureCode);

        var response = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeGameInterfaceJDBClient.GameUrlAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.Domain),
                Arg.Is(configuration.AesKey),
                Arg.Is(configuration.AesIV),
                Arg.Is(player.PlayerCode),
                Arg.Is("7"),
                Arg.Is("1"),
                Arg.Is(gameProviderCultureCode)
            ).Returns(response);

        var sut = new JDBTransferGameUrlGetService(
            fakeGameInterfaceJDBClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService,
            fakeGameRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri("http://localhost/"), actual);
    }
}
