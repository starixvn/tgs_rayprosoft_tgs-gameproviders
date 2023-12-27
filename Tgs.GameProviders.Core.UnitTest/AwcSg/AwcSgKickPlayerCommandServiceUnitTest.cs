using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Core.AwcSg.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcSg;

public class AwcSgKickPlayerCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var command = new AwcSgKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_SG),
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
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new AwcSgConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
            BetLimit = new JsonObject
            {
                ["SV388"] = new JsonObject
                {
                    ["LIVE"] = new JsonObject
                    {
                        ["maxbet"] = 2400,
                        ["minbet"] = 1,
                        ["mindraw"] = 1,
                        ["matchlimit"] = 4800,
                        ["maxdraw"] = 1200
                    }
                }
            }
        };
        _ = fakeGameProvider
            .GetAwcSgConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var sut = new AwcSgKickPlayerCommandService(
            fakeBrand,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        await sut.ExecuteAsync(command);

        _ = fakeClient
            .Received(1)
            .PlayerLogoutAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            cert: Arg.Is(configuration.Cert),
            playerCode: Arg.Is(command.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
