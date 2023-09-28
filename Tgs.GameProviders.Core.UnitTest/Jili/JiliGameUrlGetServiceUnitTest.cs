using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Core.Jili.ApplicationServices;
using Tgs.GameProviders.Core.Jili.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Jili;

namespace Tgs.GameProviders.Core.UnitTest.Jili;

public class JiliGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeTokenProvider = Substitute.For<ITokenProvider>();
        var fakeService = Substitute.For<IJiliService>();
        var fakeMerchantMapper = Substitute.For<IJiliMerchantMapper>();
        var fakeApiClient = Substitute.For<IJiliApiClient>();

        var gameUrl = new Uri("http://www.google.com");
        var token = "token";
        var gameProviderLanguageCode = "en-US";

        var merchant = new JiliMerchant
        {
            BrandCode = DefConst.BrandCodeDefault,
            AgentId = "PG_Seamless",
            AgentKey = "0d90dda822b6df548460da8f551c1a76c01b6a9e"
        };

        var configuration = new JiliConfiguration
        {
            ApiUrl = new Uri("https://uat-wb-api.jlfafafa2.com"),
            Merchants = new[]
            {
                merchant
            },
            ClientId = "ClientId",
            SecretKey = "SecretKey"
        };

        var gameInfo = new GameInfo(
            GameId: 2200001,
            GameCode: "test",
            GameProviderId: Guid.Parse("b957e28c-0e94-40af-9d37-ab48cdeb5d29"),
            GameProviderCode: GameProviderType.Jili,
            GameExternalId: "10",
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
            SubGameProviderCode: GameProviderType.Jili);

        var query = new JiliGameUrlQuery
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

        var sut = new JiliGameUrlGetService(
            fakeGameRepository,
            fakeTokenProvider,
            fakeService,
            fakeMerchantMapper,
            fakeApiClient);

        _ = fakeTokenProvider.EncryptAsync(Arg.Any<string>()).Returns(ValueTask.FromResult(token));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeMerchantMapper.GetMerchant(configuration.Merchants, query.BrandCode).Returns(merchant);
        _ = fakeGameRepository.GetAsync(gameInfo.GameId).Returns(ValueTask.FromResult<GameInfo>(gameInfo));
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        _ = fakeApiClient
            .LoginWithoutRedirectAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                new()
                {
                    Token = token,
                    GameExternalId = gameInfo.GameExternalId,
                    Language = query.Language
                })
            .Returns(ValueTask.FromResult(
                new JiliLoginWithoutRedirectResponse
                {
                    ErrorCode = 0,
                    Url = gameUrl.AbsoluteUri
                }));

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual.Url);
    }
}
