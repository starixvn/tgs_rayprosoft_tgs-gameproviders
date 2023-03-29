using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EndorphinaService.Protos;
using Tgs.GameProviders.Core.Endorphina.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRecord.Endorphina.Session.Client;

namespace Tgs.GameProviders.Core.UnitTest.Endorphina;

[TestFixture]
public class EndorphinaGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeGameInterfaceEndorphinaClient = Substitute.For<IGameInterfaceEndorphinaClient>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeSessionClient = Substitute.For<IGameRecordEndorphinaSessionClient>();

        var gameId = 5500019;

        var query = new EndorphinaGameUrlQuery
        {
            PlayerCode = "4ucmqplxb029",
            GameId = gameId,
            CurrencyCode = "CNY",
            Language = "en-US",
            BackUrl = new Uri("http://localhost")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.Endorphina)).Returns(gameProviderId);

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

        var configuration = new EndorphinaConfiguration()
        {
            ApiUrl = new("https://test.endorphina.network"),
            MerchantId = "1494",
            SecretKey = "9BC9E97FFCD74B16B87BD6FA82C8C2AF",
        };
        _ = fakeGameProviderRepository.GetEndorphinaConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var gameInfo = new GameInfo(
            GameId: gameId,
            GameCode: "Ninja",
            GameProviderId: Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            GameProviderCode: "ENDOR",
            GameExternalId: "endorphina_Ninja",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Ninja",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "ENDOR");

        _ = fakeGameRepository.GetAsync(gameId, Arg.Any<CancellationToken>()).Returns(gameInfo);

        _ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, query.CurrencyCode).Returns("CNY");

        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, query.Language).Returns("en");

        var response = new GameTokenReply()
        {
            Token = "96E108A557044C5EB0D0EF5EFED9596B",
            Game = "endorphina_Ninja",
            Username = "4ucmqplxb029",
        };

        _ = fakeGameInterfaceEndorphinaClient
            .GameTokenAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.MerchantId),
                Arg.Is(configuration.SecretKey),
                Arg.Is(gameInfo.GameExternalId),
                Arg.Is("CNY"),
                Arg.Is(player.PlayerCode),
                Arg.Any<CancellationToken>())
            .Returns(response);

        var sut = new EndorphinaGameUrlGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceEndorphinaClient,
            fakePlayerRepository,
            fakeGameRepository,
            fakeSessionClient);

        var expected = "https://test.endorphina.network/icasino/sessions/launch?token=96E108A557044C5EB0D0EF5EFED9596B&sign=0A79E18AD2D92F03273AF6D44DF270FE&lang=en&exit=https://test.endorphina.network/icasino/sessions/launch?token=96E108A557044C5EB0D0EF5EFED9596B&sign=0A79E18AD2D92F03273AF6D44DF270FE&lang=en";

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri(expected), actual);
    }
}
