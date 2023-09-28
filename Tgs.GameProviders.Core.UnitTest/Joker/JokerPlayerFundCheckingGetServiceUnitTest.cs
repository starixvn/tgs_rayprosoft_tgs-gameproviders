using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

public class JokerPlayerFundCheckingGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceJokerClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new JokerPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
        .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.Joker),
            cancellationToken: Arg.Any<CancellationToken>())
        .Returns(gameProviderId);

        var playerModel = new PlayerInfo(
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
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerModel);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                brandId: Arg.Is(playerModel.BrandId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new JokerConfiguration
        {
            ApiUrl = new("http://localhost/"),
            GameLaunchUrl = new Uri("http://localhost/"),
            AppId = "appId",
            SecretKey = "secretKey"
        };
        _ = fakeGameProvider
            .GetJokerConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        _ = fakeClient
            .FundStatusAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                appId: Arg.Is(configuration.AppId),
                secretKey: Arg.Is(configuration.SecretKey),
                transactionId: Arg.Is(query.OrderId))
            .Returns(true);

        var sut = new JokerPlayerFundCheckingGetService(
            fakeBrand,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        var actual = await sut.GetAsync(query);

        Assert.True(actual);
    }
}
