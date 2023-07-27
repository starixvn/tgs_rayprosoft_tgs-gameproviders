using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.BTiService.Protos;
using Tgs.GameProviders.Core.BTiSports.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.BTiSports;

public class BTiSportsGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceBTiClient = Substitute.For<IGameInterfaceBTiClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new BTiSportsGameUrlQuery
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
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.BTiSports)).Returns(gameProviderId);

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

        var configuration = new BTiSportsConfiguration()
        {
            ApiUrl = new("https://wallet.bti.bet/"),
            TokenApiUrl = new("https://asia-east2-bq-data-api.cloudfunctions.net/"),
            HistoryBetsApiUrl = new("https://get-bet-by-id-d5nlg5buca-de.a.run.app/"),
            HistoryBetsPagingApiUrl = new("https://get-history-bets-paging-d5nlg5buca-de.a.run.app/"),
            OpenBetsApiUrl = new("https://get-open-bets-d5nlg5buca-de.a.run.app/"),
            DesktopLayout = "sports",
            MobileLayout = "sports",
            AgentUserName = "PegaGaming_GPIRMB",
            AgentPassword = "IM60Ho7Y4B9C",
            SportsbookURL = new("https://prod20240-108707434.1x2aaa.com")
        };
        _ = fakeGameProviderRepository.GetBTiSportsConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var cultureCode = "zh";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, query.Language).Returns(cultureCode);

        var response = new GameAuthTokenReply
        {
            IsPlayerExist = true,
            AuthToken = Guid.NewGuid().ToString("N")
        };
        _ = fakeGameInterfaceBTiClient.GameAuthTokenAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.AgentUserName),
                Arg.Is(configuration.AgentPassword),
                Arg.Is(player.PlayerCode)).Returns(response);

        var sut = new BTiSportsGameUrlGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceBTiClient,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(new Uri(configuration.SportsbookURL, $"/{cultureCode}/{configuration.DesktopLayout}/?operatorToken={response.AuthToken}"), actual);
    }
}
