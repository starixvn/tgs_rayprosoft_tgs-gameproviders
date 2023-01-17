using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferGameUrlGetServiceUnitTest
{
    [Test]
    public void Create_Player()
    {
        var fakePragmaticPlayTransferApiClient = Substitute.For<IPragmaticPlayTransferGamePlayer>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();

        var gameProviderLanguageCode = "en";

        var configuration = new PragmaticPlayTransferConfiguration()
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.PragmaticPlayTransfer,
            GameExternalId: "1",
            Type: GameType.LiveDealerLobby,
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
            SubGameProviderCode: GameProviderType.PragmaticPlayTransfer);

        var query = new PragmaticPlayTransferGameUrlQuery
        {
            GameType = gameInfo.Type,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var sut = new PragmaticPlayTransferGameUrlGetService(
            fakePlayerRepository,
            fakeGameRepository,
            fakeService,
            fakeApiClient,
            fakePragmaticPlayTransferApiClient);

        _ = fakeGameRepository.GetAsync(Arg.Is(gameInfo.GameId)).Returns(gameInfo);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService
             .GetGameProviderLanguageCodeAsync(Arg.Is(gameInfo.GameProviderId), Arg.Is(query.Language))
             .Returns(gameProviderLanguageCode);

        var player = new PlayerInfo(
                Guid.NewGuid(),
                "TGS",
                Guid.NewGuid(),
                "TGS",
                Guid.NewGuid(),
                "GPI",
                query.PlayerCode,
                query.PlayerCode,
                Guid.NewGuid(),
                "CNY");
        _ = fakePlayerRepository
            .GetAsync(
                Arg.Is(query.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeService
            .GetGameProviderIdAsync()
            .Returns(gameProviderId);

        var gameProviderCurrencyCode = "1";
        _ = fakeService
            .GetGameProviderCurrencyCodeAsync(
                Arg.Is(gameProviderId),
                Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(gameProviderCurrencyCode);

        _ = fakeApiClient
            .GetGameUrlAsync(
                 Arg.Is(configuration.ApiUrl),
                 Arg.Is(configuration.SecureLogin),
                 Arg.Is(configuration.SecretKey),
                 Arg.Is(query.PlayerCode),
                 Arg.Is(gameInfo.GameExternalId),
                 Arg.Is(gameProviderLanguageCode),
                 Arg.Any<string>(),
                 Arg.Is(query.BackUrl.OriginalString))
             .Returns(ValueTask.FromResult<(bool IsPlayerExist, string Url)>(new(false, string.Empty)));

        _ = Assert.ThrowsAsync<Exception>(() => sut.GetAsync(query).AsTask());

        _ = fakePragmaticPlayTransferApiClient
            .Received(1)
            .CreatePlayerAsync(
                query.PlayerCode,
                gameProviderCurrencyCode,
                Arg.Any<PragmaticPlayTransferConfiguration>()
            );
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakePragmaticPlayTransferApiClient = Substitute.For<IPragmaticPlayTransferGamePlayer>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();

        var gameUrl = new Uri("https://tgs.prerelease-env.biz/gs2c/openGame.do?tc=97YBxmEGn5PCQv1zxvHGDlfRWeqGsKxLc2K1HsdyJKiIDck46D5azpqgW0Be3PrO&stylename=tgs_tgstfw&dummy=");

        var gameProviderLanguageCode = "en";

        var configuration = new PragmaticPlayTransferConfiguration()
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.PragmaticPlayTransfer,
            GameExternalId: "1",
            Type: GameType.LiveDealerLobby,
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
            SubGameProviderCode: GameProviderType.PragmaticPlayTransfer);

        var query = new PragmaticPlayTransferGameUrlQuery
        {
            GameType = gameInfo.Type,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var sut = new PragmaticPlayTransferGameUrlGetService(
            fakePlayerRepository,
            fakeGameRepository,
            fakeService,
            fakeApiClient,
            fakePragmaticPlayTransferApiClient);

        _ = fakeGameRepository.GetAsync(Arg.Is(gameInfo.GameId)).Returns(gameInfo);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService
             .GetGameProviderLanguageCodeAsync(Arg.Is(gameInfo.GameProviderId), Arg.Is(query.Language))
             .Returns(gameProviderLanguageCode);

        _ = fakeApiClient
            .GetGameUrlAsync(
                 Arg.Is(configuration.ApiUrl),
                 Arg.Is(configuration.SecureLogin),
                 Arg.Is(configuration.SecretKey),
                 Arg.Is(query.PlayerCode),
                 Arg.Is(gameInfo.GameExternalId),
                 Arg.Is(gameProviderLanguageCode),
                 Arg.Any<string>(),
                 Arg.Is(query.BackUrl.OriginalString))
             .Returns(ValueTask.FromResult<(bool IsPlayerExist, string Url)>(new(true, gameUrl.OriginalString)));

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
