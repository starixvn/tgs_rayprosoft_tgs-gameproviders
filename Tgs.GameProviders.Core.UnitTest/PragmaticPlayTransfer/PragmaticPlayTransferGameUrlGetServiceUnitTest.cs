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
        var fakeGameService = Substitute.For<IGameService>();
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

        var game = new GameInfo(
            2700001,
            GameStatus.Active,
            "vs20kraken",
            null,
            null,
            null,
            Guid.Parse("E4ECFA98-B373-4123-87E0-CB9E883D0691"),
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
            fakeGameService,
            fakeService,
            fakeApiClient);

        _ = fakeGameService.GetCacheAsync(Arg.Is(game.GameId)).Returns((GameInfo?)game);
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
             .Returns(gameUrl.OriginalString);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}