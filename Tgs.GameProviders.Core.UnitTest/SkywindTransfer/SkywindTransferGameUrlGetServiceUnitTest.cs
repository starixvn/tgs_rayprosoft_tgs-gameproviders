using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameToken.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.SkywindTransfer;

[TestFixture]
public class SkywindTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeCreatePlayerCommand = Substitute.For<ICommandService<SkywindTransferCreatePlayerCommand>>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceSkywindClient = Substitute.For<IGameInterfaceSkywindClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();
        var fakeGameTokenClient = Substitute.For<IGameTokenClient>();

        var configuration = new SkywindTransferConfiguration
        {
            ApiUrl = new("http://localhost/"),
            LiveLobbyUrl = new("http://localhost/"),
            MerchId = "merchId",
            MerchPwd = "merchPwd",
            SecretKey = "secretKey",
            Username = "username",
            Password = "password"
        };

        var query = new SkywindTransferGameUrlQuery
        {
            GameType = GameType.Slot,
            GameId = 1,
            BrandCode = "GPI",
            PlayerCode = "test",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("http://localhost/")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SkywindTransfer)).Returns(ValueTask.FromResult(gameProviderId));

        _ = fakeSettingRepository.GetSkywindTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var playerModel = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

        var cultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
            Arg.Is(gameProviderId),
            Arg.Is(query.Language)).Returns(cultureCode);

        var currencyCode = "CNY";
        _ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
            Arg.Is(gameProviderId),
            Arg.Is(query.CurrencyCode)).Returns(currencyCode);

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.SkywindTransfer,
            GameExternalId: "10",
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
            SubGameProviderCode: GameProviderType.SkywindTransfer);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameInfo);

        var request = new GameUrlRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            PlayerCode = query.PlayerCode,
            GameCode = gameInfo.GameExternalId,
            Language = cultureCode
        };
        var response = new GameUrlReply()
        {
            IsPlayer = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeGameInterfaceSkywindClient.GameUrlAsync(
            Arg.Is(request)).Returns(response);

        var sut = new SkywindTransferGameUrlGetService(
            fakeCreatePlayerCommand,
            fakeGameRepository,
            fakeGameProviderRepository,
            fakeGameInterfaceSkywindClient,
            fakePlayerRepository,
            fakeSettingRepository,
            fakeGameTokenClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri("http://localhost/"), actual.Url);
    }
}
