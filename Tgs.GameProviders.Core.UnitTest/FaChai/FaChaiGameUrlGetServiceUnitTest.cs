using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Core.FaChai.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.FaChai;

public class FaChaiGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeClient = Substitute.For<IGameInterfaceFaChaiClient>();

        var query = new FaChaiGameUrlQuery
        {
            GameType = GameType.Slot,
            GameId = 1,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "zh-CN",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.FaChai),
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
        _ = fakePlayerRepository
            .GetAsync(
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
                 gameProviderId: Arg.Is(gameProviderId),
                 brandId: Arg.Is(player.BrandId),
                 cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new FaChaiConfiguration()
        {
            ApiUrl = new("https://api.dg0.co/"),
            AgentCode = "GPICN",
            AgentKey = "EOJC9q31Y7AnPb9i",
            Currency = "CNY"
        };
        _ = fakeGameProviderRepository
            .GetFaChaiConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var cultureCode = "1";
        _ = fakeGameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                cultureCode: Arg.Is(query.Language),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var game = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.FaChai,
            GameExternalId: "1",
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
            SubGameProviderCode: GameProviderType.FaChai);
        _ = fakeGameRepository
            .GetAsync(
                gameId: Arg.Is(query.GameId.Value),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameUrl = "https://web.fcg666.net/index?Params=eyJpdiI6IndFU3VWOVdsV0MvWG1aNGlYS2RVdHc9PSIsInZhbHVlIjoiVTIzOE00ZUJ0YjJGaXVDbnRuajZUNDBXdFVXc0dMYlZKQllKbXN2Mm1aNTJqOXN1UGg4ZHJyS0ZBZnN6VTJsWlFpVUFTeE5kdFFCa3VCM3RDU00xNHMrRTNwd3pKOGdVRVFsSGphN1NTMXlKem15Mmc2aGMxTFdwNC9ERUU4ZFByczhGdTYzemNGNjAxb29mUHd3d3dnPT0iLCJtYWMiOiIzYWUwMWJjZDJlYzcxYzdmMmJjNmVjMzY0ZWM1YTU3ODc2OGQ4MjMzZjAxZTExYzkxMWE3MDVhZjkxODNhODA1In0%3D&languageID=2";

        _ = fakeClient
            .GameUrlAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                agentCode: Arg.Is(configuration.AgentCode),
                agentKey: Arg.Is(configuration.AgentKey),
                currency: Arg.Is(configuration.Currency),
                playerCode: Arg.Is(player.PlayerCode),
                gameId: Arg.Is(int.Parse(game.GameExternalId)),
                languageId: Arg.Is(int.Parse(cultureCode)),
                Arg.Any<CancellationToken>())
            .Returns(gameUrl);

        var sut = new FaChaiGameUrlGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeGameRepository,
            fakeClient);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(new Uri(gameUrl), actual);
    }
}
