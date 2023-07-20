using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcAelotto;

[TestFixture]
public class AwcAelottoGameUrlGetServiceUnitTest
{
    [Test]
    public void Create_Player()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcAelottoGameUrlQuery
        {
            GameType = GameType.LotteryLobby,
            GameId = 2900001,
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
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_AELOTTO),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

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
        _ = fakePlayer
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new AwcAelottoConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcAelottoConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var gameProviderCultureCode = "en";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            cultureCode: Arg.Is(query.Language),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderCultureCode);

        var game = new GameInfo(
            GameId: 2800015,
            GameCode: "YeeKee_Lotto",
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_AELOTTO,
            GameExternalId: "AE-LOTTO-021",
            Type: GameType.Lottery,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "YeeKee Lotto",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "AWC_YEEKEE_LOTTO");
        _ = fakeGame
            .GetAsync(
            Arg.Is(query.GameId.Value),
            Arg.Any<CancellationToken>())
            .Returns(game);

        var request = new GameUrlRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            PlayerCode = query.PlayerCode,
            BackUrl = query.BackUrl.ToString(),
            IsMobileLogin = query.PlatformType == PlatformType.Mobile,
            ExternalGameId = "AE-LOTTO-021",
            Language = gameProviderCultureCode
        };
        var gameUrl = new Uri("http://www.google.com");
        var response = new GameUrlReply()
        {
            Success = true,
            IsPlayerExist = false,
            GameUrl = string.Empty
        };
        _ = fakeClient.GameUrlByAwcAeLottoAsync(request).Returns(response);

        var currency = "CNY";
        _ = fakeGameProvider
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currency);

        var sut = new AwcAelottoGameUrlGetService(
            fakeBrand,
            fakeGame,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        _ = Assert.ThrowsAsync<UriFormatException>(async ()
            => await sut
            .GetAsync(
                query: query)
            .ConfigureAwait(false));

        _ = fakeClient
            .Received(1)
            .CreatePlayerAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            cert: Arg.Is(configuration.Cert),
            playerCode: Arg.Is(query.PlayerCode),
            userName: Arg.Is(query.PlayerCode),
            currency: Arg.Is(currency),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task Normal_Process_SubGP_AWC_AELOTTO()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcAelottoGameUrlQuery
        {
            GameType = GameType.LotteryLobby,
            GameId = 2900001,
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
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_AELOTTO),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

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
        _ = fakePlayer
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new AwcAelottoConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcAelottoConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var gameProviderCultureCode = "en";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            cultureCode: Arg.Is(query.Language),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderCultureCode);

        var game = new GameInfo(
            GameId: 2800003,
            GameCode: "Thai_Shiny_Lottery",
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_AELOTTO,
            GameExternalId: "AE-LOTTO-001",
            Type: GameType.Lottery,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Thai Shiny Lottery",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.AWC_AELOTTO);
        _ = fakeGame
            .GetAsync(
            Arg.Is(query.GameId.Value),
            Arg.Any<CancellationToken>())
            .Returns(game);

        var request = new GameUrlRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            PlayerCode = query.PlayerCode,
            BackUrl = query.BackUrl.ToString(),
            IsMobileLogin = query.PlatformType == PlatformType.Mobile,
            ExternalGameId = "AE-LOTTO-004",
            Language = gameProviderCultureCode
        };
        var gameUrl = new Uri("http://www.google.com");
        var response = new GameUrlReply()
        {
            Success = true,
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };
        _ = fakeClient.GameUrlByAwcAeLottoAsync(request).Returns(response);

        var sut = new AwcAelottoGameUrlGetService(
            fakeBrand,
            fakeGame,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }

    [Test]
    public async Task Normal_Process_SubGP_AWC_YEEKEE_LOTTO()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcAelottoGameUrlQuery
        {
            GameType = GameType.LotteryLobby,
            GameId = 2900001,
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
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_AELOTTO),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

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
        _ = fakePlayer
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new AwcAelottoConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcAelottoConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var gameProviderCultureCode = "en";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            cultureCode: Arg.Is(query.Language),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderCultureCode);

        var game = new GameInfo(
            GameId: 2800015,
            GameCode: "YeeKee_Lotto",
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_AELOTTO,
            GameExternalId: "AE-LOTTO-021",
            Type: GameType.Lottery,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "YeeKee Lotto",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "AWC_YEEKEE_LOTTO");
        _ = fakeGame
            .GetAsync(
            Arg.Is(query.GameId.Value),
            Arg.Any<CancellationToken>())
            .Returns(game);

        var request = new GameUrlRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            PlayerCode = query.PlayerCode,
            BackUrl = query.BackUrl.ToString(),
            IsMobileLogin = query.PlatformType == PlatformType.Mobile,
            ExternalGameId = "AE-LOTTO-021",
            Language = gameProviderCultureCode
        };
        var gameUrl = new Uri("http://www.google.com");
        var response = new GameUrlReply()
        {
            Success = true,
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };
        _ = fakeClient.GameUrlByAwcAeLottoAsync(request).Returns(response);

        var sut = new AwcAelottoGameUrlGetService(
            fakeBrand,
            fakeGame,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
