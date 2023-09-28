using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameInterface.AVIAGaming.Protos;
using Tgs.GameProviders.Core.AVIAGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AVIAGaming;

public class AVIAGamingPlayerFundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAVIAGamingClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new AVIAGamingPlayerFundOutQuery()
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 1.00m
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

        var currencyCode = "CNY";
        _ = fakeGameProvider
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currencyCode);

        var fundReply = new FundReply
        {
            OrderId = query.OrderId,
            Resp = new JsonObject { }.ToJsonString()
        };
        _ = fakeClient
            .FundOutAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            playerCode: Arg.Is(player.PlayerCode),
            currency: Arg.Is(currencyCode),
            orderId: query.OrderId,
            amount: query.Amount,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(fundReply);

        var sut = new AVIAGamingPlayerFundOutGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Equal(fundReply.OrderId, actual.OrderId);
    }
}
