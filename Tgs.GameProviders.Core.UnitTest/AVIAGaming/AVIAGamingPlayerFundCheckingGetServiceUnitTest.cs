using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameProviders.Core.AVIAGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AVIAGaming;

public class AVIAGamingPlayerFundCheckingGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAVIAGamingClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new AVIAGamingPlayerFundCheckingQuery()
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = "018976fec9540000000107e724d16f8c"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AVIAGaming),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderId));

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
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
            .Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configuration = new AVIAGamingConfiguration
        {
            AmountMaximum = 10000000,
            AmountFormat = "10,2",
            ApiUrl = new Uri("http://localhost/"),
            Authorization = "authorization",
            PlayerPassword = "TgsP@ssw0rd"
        };
        _ = fakeGameProvider
            .GetAVIAGamingConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var status = true;
        _ = fakeClient
            .FundStatusAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            orderId: Arg.Is(query.OrderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(status);

        var sut = new AVIAGamingPlayerFundCheckingGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.True(actual);
    }
}
