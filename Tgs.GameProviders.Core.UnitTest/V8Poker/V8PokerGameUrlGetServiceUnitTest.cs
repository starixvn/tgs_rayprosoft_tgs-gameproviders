using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameProviders.Core.V8Poker.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.V8Poker;

public class V8PokerGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceV8PokerClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGame = Substitute.For<IGameRepository>();

        var query = new V8PokerGameUrlQuery
        {
            GameType = GameType.CardGameLobby,
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
        _ = fakeGameProvider.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.V8Poker),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderId));

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "HKD");
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
            .Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configuration = new V8PokerConfiguration
        {
            AmountMaximum = 999999999999,
            AmountFormat = "12,2",
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
            Deskey = "deskey",
            Md5key = "md5key",
            AgentId = "200995"
        };
        _ = fakeGameProvider
            .GetV8PokerConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.V8Poker,
            GameExternalId: "0",
            Type: GameType.CardGameLobby,
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
            SubGameProviderCode: GameProviderType.V8Poker);
        _ = fakeGame
            .GetAsync(
                Arg.Is(query.GameId.Value),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameInfo);

        var cultureCode = "en-us";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                cultureCode: Arg.Is(query.Language),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var gameurl = "https://localhost";
        _ = fakeClient
            .GameUrlAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            deskey: Arg.Is(configuration.Deskey),
            md5key: Arg.Is(configuration.Md5key),
            agentId: Arg.Is(configuration.AgentId),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            ipAddress: Arg.Is(query.IPAddress),
            externalGameId: Arg.Is(gameInfo.GameExternalId),
            lang: Arg.Is(cultureCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var sut = new V8PokerGameUrlGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeGame,
            fakeClient);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameurl, actual);
    }
}
