using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcRcb.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcRcb;

[TestFixture]
public class AwcRcbGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcRcbGameUrlQuery
        {
            GameType = GameType.LiveDealerLobby,
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
            gameProviderCode: Arg.Is(GameProviderType.AWC_RCB),
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

        var configuration = new AwcRcbConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
            BetLimit = new JsonObject
            {
                ["HORSEBOOK"] = new JsonObject
                {
                    ["LIVE"] = new JsonObject
                    {
                        ["minbet"] = 10,
                        ["maxbet"] = 3000,
                        ["maxBetSumPerHorse"] = 10000,
                        ["minorMinbet"] = 10,
                        ["minorMaxbet"] = 1000,
                        ["minorMaxBetSumPerHorse"] = 10000
                    }
                }
            }
        };
        _ = fakeGameProvider
            .GetAwcRcbConfigurationAsync(
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

        var request = new GameUrlByLiveLobbyRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            PlayerCode = query.PlayerCode,
            BackUrl = query.BackUrl.ToString(),
            IsMobileLogin = query.PlatformType == PlatformType.Mobile,
            BetLimit = configuration.BetLimit.ToJsonString(),
            Language = gameProviderCultureCode
        };
        var gameUrl = new Uri("http://www.google.com");
        var response = new GameUrlReply()
        {
            Success = true,
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };
        _ = fakeClient.GameUrlLiveLobbyByAwcRcbAsync(request).Returns(response);

        var sut = new AwcRcbGameUrlGetService(
            fakeBrand,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
