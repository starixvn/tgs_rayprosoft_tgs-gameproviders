using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.OBet.Protos;
using Tgs.GameProviders.Core.OBet.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.OBet;

public class OBetGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfaceOBetClient>();

        var query = new OBetGameUrlQuery
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
                gameProviderCode: Arg.Is(GameProviderType.OBet),
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

        var configuration = new OBetConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiKey = "apiKey",
            AgentId = "agentId",
            MaxBetGrpHdp = 269m,
            MaxBetGrpBasketBall = 169m,
            MaxBetGrpOthersSport = 69m,
            MaxBetGrp1x2 = 299m,
            MaxBetGrpMixParlay = 199m,
            MaxBetGrpOthersOdds = 109m,
            MaxBetGrpSpecial = 59m
        };
        _ = fakeGameProviderRepository
            .GetOBetConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var cultureCode = "en";
        _ = fakeGameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                cultureCode: Arg.Is(query.Language),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var gameUrlReply = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "http://localhost/"
        };

        _ = fakeClient
            .GameUrlAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                apiKey: Arg.Is(configuration.ApiKey),
                agentId: Arg.Is(configuration.AgentId),
                playerCode: Arg.Is(player.PlayerCode),
                lang: Arg.Is(cultureCode),
                isMobile: Arg.Is(query.PlatformType == PlatformType.Mobile),
                Arg.Any<CancellationToken>())
            .Returns(gameUrlReply);

        var sut = new OBetGameUrlGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(new Uri(gameUrlReply.GameUrl), actual);

        await fakeClient
            .Received(1)
            .PlayerUpdateBetLimitAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                apiKey: Arg.Is(configuration.ApiKey),
                agentId: Arg.Is(configuration.AgentId),
                playerCode: Arg.Is(player.PlayerCode),
                maxBetGrpHdp: Arg.Is(configuration.MaxBetGrpHdp),
                maxBetGrpBasketBall: Arg.Is(configuration.MaxBetGrpBasketBall),
                maxBetGrpOthersSport: Arg.Is(configuration.MaxBetGrpOthersSport),
                maxBetGrp1x2: Arg.Is(configuration.MaxBetGrp1x2),
                maxBetGrpMixParlay: Arg.Is(configuration.MaxBetGrpMixParlay),
                maxBetGrpOthersOdds: Arg.Is(configuration.MaxBetGrpOthersOdds),
                maxBetGrpSpecial: Arg.Is(configuration.MaxBetGrpSpecial),
                Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
