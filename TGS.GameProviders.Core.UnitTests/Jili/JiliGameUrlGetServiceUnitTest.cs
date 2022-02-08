using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Jili.ApplicationServices;
using TGS.GameProviders.Core.Jili.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Jili;

namespace TGS.GameProviders.Core.UnitTests.Jili;

[TestFixture]
public class JiliGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<JiliGameUrlGetService>.Instance;
        var fakeGameService = Substitute.For<IGameService>();
        var fakeTokenProvider = Substitute.For<ITokenProvider>();
        var fakeService = Substitute.For<IJiliService>();
        var fakeMerchantMapper = Substitute.For<IJiliMerchantMapper>();
        var fakeApiClient = Substitute.For<IJiliApiClient>();

        var gameUrl = new Uri("http://www.google.com");
        var token = "token";
        var gameProviderLanguageCode = "en-US";

        var merchant = new JiliMerchant
        {
            BrandCode = "Default",
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

        var game = new GameInfo(
            2200001,
            GameStatus.Active,
            "10",
            null,
            null,
            null,
            Guid.Parse("b957e28c-0e94-40af-9d37-ab48cdeb5d29"),
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
            fakeLogger,
            fakeGameService,
            fakeTokenProvider,
            fakeService,
            fakeMerchantMapper,
            fakeApiClient);

        fakeTokenProvider.EncryptAsync(Arg.Any<string>()).Returns(ValueTask.FromResult(token));
        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeMerchantMapper.GetMerchant(configuration.Merchants, query.BrandCode).Returns(merchant);
        fakeGameService.GetCacheAsync(game.GameId).Returns(ValueTask.FromResult(game));
        fakeService
            .GetGameProviderLanguageCodeAsync(game.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakeApiClient
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
