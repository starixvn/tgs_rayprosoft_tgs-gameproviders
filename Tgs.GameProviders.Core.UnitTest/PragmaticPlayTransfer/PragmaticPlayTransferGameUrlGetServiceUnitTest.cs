using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferGameUrlGetServiceUnitTest
{
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

        var game = new GameModel(
            2700001,
              Guid.Parse("E4ECFA98-B373-4123-87E0-CB9E883D0691"),
              "PPTF",
            "vs20kraken",
            GameStatus.Active,
            null,
            null,
            null,
            GameType.Slot,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Release_the_Kraken",
            "Release the Kraken",
            null,
            "PPTF");

        var query = new PragmaticPlayTransferGameUrlQuery
        {
            GameType = game.GameType,
            GameId = game.GameId,
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

        _ = fakeGameRepository.GetAsync(Arg.Is(game.GameId)).Returns((GameModel?)game);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService
             .GetGameProviderLanguageCodeAsync(Arg.Is(game.GameProviderId), Arg.Is(query.Language))
             .Returns(gameProviderLanguageCode);

        _ = fakeApiClient
            .GetGameUrlAsync(
                 Arg.Is(configuration.ApiUrl),
                 Arg.Is(configuration.SecureLogin),
                 Arg.Is(configuration.SecretKey),
                 Arg.Is(query.PlayerCode),
                 Arg.Is(game.GameExternalId),
                 Arg.Is(gameProviderLanguageCode),
                 Arg.Any<string>(),
                 Arg.Is(query.BackUrl.OriginalString))
             .Returns(ValueTask.FromResult<(bool IsPlayerExist, string Url)>(new(true, gameUrl.OriginalString)));

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }

    [Test]
    public async Task Create_Player()
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

        var game = new GameModel(
            2700001,
              Guid.Parse("E4ECFA98-B373-4123-87E0-CB9E883D0691"),
              "PPTF",
            "vs20kraken",
            GameStatus.Active,
            null,
            null,
            null,
            GameType.Slot,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Release_the_Kraken",
            "Release the Kraken",
            null,
            "PPTF");

        var query = new PragmaticPlayTransferGameUrlQuery
        {
            GameType = game.GameType,
            GameId = game.GameId,
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

        _ = fakeGameRepository.GetAsync(Arg.Is(game.GameId)).Returns((GameModel?)game);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService
             .GetGameProviderLanguageCodeAsync(Arg.Is(game.GameProviderId), Arg.Is(query.Language))
             .Returns(gameProviderLanguageCode);

        var player = new PlayerModel(
                Guid.NewGuid(),
                "TGS",
                Guid.NewGuid(),
                "TGS",
                Guid.NewGuid(),
                "GPI",
                query.PlayerCode,
                query.PlayerCode,
                Guid.NewGuid(),
                "CNY",
                PlayerStatus.Enabled);
        _ = fakePlayerRepository
            .GetAsync(
                Arg.Is(query.PlayerCode))
            .Returns(new PlayerModel(
                Guid.NewGuid(),
                "TGS",
                Guid.NewGuid(),
                "TGS",
                Guid.NewGuid(),
                "GPI",
                query.PlayerCode,
                query.PlayerCode,
                Guid.NewGuid(),
                "CNY",
                PlayerStatus.Enabled));

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
                 Arg.Is(game.GameExternalId),
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
}
