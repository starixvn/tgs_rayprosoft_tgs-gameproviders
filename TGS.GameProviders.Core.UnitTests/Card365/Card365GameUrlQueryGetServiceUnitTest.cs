using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Core.Card365.ApplicationServices;
using TGS.GameProviders.Core.Card365.Model;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Card365;

namespace TGS.GameProviders.Core.UnitTests.Card365;

[TestFixture]
public class Card365GameUrlQueryGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<Card365GameUrlQueryGetService>.Instance;
        var fakeService = Substitute.For<ICard365Service>();
        var fakeApiClient = Substitute.For<ICard365ApiClient>();
        var fakeAgentMapper = Substitute.For<ICard365AgentMapper>();
        var fakeGameService = Substitute.For<IGameService>();
        var fakePlayerService = Substitute.For<IPlayerService>();
        var fakeTokenProvider = Substitute.For<ITokenProvider>();

        var token = "token";
        var gameProviderCurrencyCode = "3";
        var gameProviderLanguageCode = "EN";

        var accessKey = "VzFob3BnV3I2Z3NLRHUweis1ekVEeTlmSzBNTHZPOVBjY3FidFNEekdCemJ1N0tvUlUzOHBLUDd0dEJnQWN5RA==";

        var gameUrl = new Uri($"https://g5ky3i.365gaming.cc/api/card365pega/loginV2?user=4ucmqplxb029&key={Uri.EscapeDataString("VzFob3BnV3I2Z3NLRHUweis1ekVEeTlmSzBNTHZPOVBjY3FidFNEekdCemJ1N0tvUlUzOHBLUDd0dEJnQWN5RA==")}&extension1=card0201&extension3={token}&userName=4ucmqplxb029&language=EN&jackpot=0&blockfree=1&enableReward=0&fullscreen=0");

        var agent = new Card365BrandAgent
        {
            BrandCode = "Default",
            AgentId = "card0201"
        };

        var configuration = new Card365Configuration
        {
            ServiceEndpointUrl = new("http://game-365"),
            ApiUrl = new("https://g5ky3i.365gaming.cc/api"),
            Cert = "755C24B710D9F2B4B15DAB2B489CF471",
            Website = "card365pega",
            BrandAgents = new[] { agent }
        };

        var gameModel = new GameInfo(
            1900001,
            GameStatus.Active,
            "384",
            null,
            null,
            null,
            Guid.Parse("630e822b-2635-4314-a7e4-5c7057c1a920"),
            GameType.CardGameLobby,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "CardGameLobby",
            "CardGameLobby",
            null,
            "365");

        var playerModel = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var query = new Card365GameUrlQuery
        {
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = playerModel.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        fakeService
            .GetGameProviderCurrencyCodeAsync(gameModel.GameProviderId, playerModel.DefaultWalletCurrencyCode)
            .Returns(ValueTask.FromResult(gameProviderCurrencyCode));

        fakeService
            .GetGameProviderLanguageCodeAsync(gameModel.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakePlayerService.GetCacheAsync(query.PlayerCode).Returns(Task.FromResult(playerModel));
        fakeGameService.GetCacheAsync(query.GameId!.Value).Returns(ValueTask.FromResult(gameModel));
        fakeAgentMapper.GetAgent(configuration.BrandAgents, playerModel.BrandCode).Returns(agent);
        fakeTokenProvider.EncryptAsync(query.LoginRecordId).Returns(ValueTask.FromResult(token));
        fakeApiClient
            .GetAccessKeyAsync(
                configuration.ApiUrl,
                configuration.Website,
                new Card365GetKeyRequest
                {
                    Cert = configuration.Cert,
                    PlayerCode = playerModel.PlayerCode,
                    Username = playerModel.Username,
                    AgentId = agent.AgentId,
                    CurrencyCode = gameProviderCurrencyCode
                })
            .Returns(ValueTask.FromResult(
                new Card365GetKeyResponse
                {
                    Status = 1,
                    Key = accessKey
                }));

        var sut = new Card365GameUrlQueryGetService(
            fakeLogger,
            fakePlayerService,
            fakeGameService,
            fakeService,
            fakeAgentMapper,
            fakeApiClient,
            fakeTokenProvider);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
