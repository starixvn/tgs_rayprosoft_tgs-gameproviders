using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcFs.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcFs;

public class AwcFsGameUrlGetServiceUnitTest
{
    [Fact]
    public void Create_Player()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcFsGameUrlQuery
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
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_FS),
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

        var configuration = new AwcFsConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcFsConfigurationAsync(
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
            GameId: 1,
            GameCode: "test",
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_FS,
            GameExternalId: "test",
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
            SubGameProviderCode: GameProviderType.AWC_FS);
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
            ExternalGameId = game.GameExternalId,
            Language = gameProviderCultureCode
        };
        var gameUrl = new Uri("http://www.google.com");
        var response = new GameUrlReply()
        {
            IsPlayerExist = false,
            GameUrl = string.Empty
        };
        _ = fakeClient.GameUrlByAwcFsAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            cert: Arg.Is(configuration.Cert),
            playerCode: Arg.Is(player.PlayerCode),
            isMobileLogin: Arg.Is(query.PlatformType == PlatformType.Mobile),
            backUrl: Arg.Is(query.BackUrl.ToString()),
            externalGameId: Arg.Is(game.GameExternalId),
            language: Arg.Is(gameProviderCultureCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

        var currency = "CNY";
        _ = fakeGameProvider
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currency);

        var sut = new AwcFsGameUrlGetService(
            fakeBrand,
            fakeGame,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        _ = Assert.ThrowsAsync<UriFormatException>(async () => await sut.GetAsync(query: query));

        _ = fakeClient
            .Received(1)
            .PlayerCreateAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            cert: Arg.Is(configuration.Cert),
            playerCode: Arg.Is(query.PlayerCode),
            userName: Arg.Is(query.PlayerCode),
            currency: Arg.Is(currency),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcFsGameUrlQuery
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
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_FS),
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

        var configuration = new AwcFsConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcFsConfigurationAsync(
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
            GameId: 1,
            GameCode: "test",
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_FS,
            GameExternalId: "test",
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
            SubGameProviderCode: GameProviderType.AWC_FS);
        _ = fakeGame
            .GetAsync(
            Arg.Is(query.GameId.Value),
            Arg.Any<CancellationToken>())
            .Returns(game);

        var gameUrl = new Uri("http://www.google.com");
        var response = new GameUrlReply()
        {
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };
        _ = fakeClient.GameUrlByAwcFsAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            cert: Arg.Is(configuration.Cert),
            playerCode: Arg.Is(player.PlayerCode),
            isMobileLogin: Arg.Is(query.PlatformType == PlatformType.Mobile),
            backUrl: Arg.Is(query.BackUrl.ToString()),
            externalGameId: Arg.Is(game.GameExternalId),
            language: Arg.Is(gameProviderCultureCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

        var sut = new AwcFsGameUrlGetService(
            fakeBrand,
            fakeGame,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual.Url);
    }
}
