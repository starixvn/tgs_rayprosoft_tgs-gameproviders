using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Core.Jili.ApplicationServices;
using Tgs.GameProviders.Core.Jili.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Jili;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Jili;

[TestFixture]
public class JiliGameUrlGetServiceUnitTest
{
    [Test]
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

        var game = new GameModel(
            2200001,
            Guid.Parse("b957e28c-0e94-40af-9d37-ab48cdeb5d29"),
            "JILI",
            "10",
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
            "FaFaFa",
            "FaFaFa",
            null,
            "JILI");

        var query = new JiliGameUrlQuery
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

        var sut = new JiliGameUrlGetService(
            fakeGameRepository,
            fakeTokenProvider,
            fakeService,
            fakeMerchantMapper,
            fakeApiClient);

        _ = fakeTokenProvider.EncryptAsync(Arg.Any<string>()).Returns(ValueTask.FromResult(token));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeMerchantMapper.GetMerchant(configuration.Merchants, query.BrandCode).Returns(merchant);
        _ = fakeGameRepository.GetAsync(game.GameId).Returns(ValueTask.FromResult<GameModel?>(game));
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(game.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        _ = fakeApiClient
            .LoginWithoutRedirectAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                new()
                {
                    Token = token,
                    GameExternalId = game.GameExternalId,
                    Language = query.Language
                })
            .Returns(ValueTask.FromResult(
                new JiliLoginWithoutRedirectResponse
                {
                    ErrorCode = 0,
                    Url = gameUrl.AbsoluteUri
                }));

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
