using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
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

        var configuration = new SWTransferConfiguration
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

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

        var cultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
            Arg.Is(gameProviderId),
            Arg.Is(query.Language)).Returns(cultureCode);

        var gameModel = new GameModel(
            1,
            Guid.NewGuid(),
            GameProviderType.SkywindTransfer,
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
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameModel);

        var request = new GameURLBySlotRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            PlayerCode = query.PlayerCode,
            GameCode = gameModel.GameExternalId,
            Language = cultureCode
        };
        var response = new GameURLReply()
        {
            IsPlayer = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeGameInterfaceSkywindClient.GameURLBySlotAsync(
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
