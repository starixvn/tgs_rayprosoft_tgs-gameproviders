using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameInterface.AVIAGaming.Protos;
using Tgs.GameProviders.Core.AVIAGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AVIAGaming;

[TestFixture]
public class AVIAGamingPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Create_PlayerAsync()
    {
        var fakeClient = Substitute.For<IGameInterfaceAVIAGamingClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new AVIAGamingPlayerBalanceQuery()
        {
            PlayerCode = "4ucmqplxb029"
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

        var gameurl = new PlayerBalanceReply
        {
            IsPlayerExist = false,
            Balance = 0
        };
        _ = fakeClient
            .PlayerBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            playerCode: Arg.Is(player.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var currency = "CNY";
        _ = fakeGameProvider
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currency);

        var sut = new AVIAGamingPlayerBalanceGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        _ = await sut
           .GetAsync(
           query: query)
           .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .PlayerCreateAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            playerCode: Arg.Is(player.PlayerCode),
            currency: Arg.Is(currency),
            password: Arg.Is(configuration.PlayerPassword),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAVIAGamingClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new AVIAGamingPlayerBalanceQuery()
        {
            PlayerCode = "4ucmqplxb029"
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

        var gameurl = new PlayerBalanceReply
        {
            IsPlayerExist = true,
            Balance = 1
        };
        _ = fakeClient
            .PlayerBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            playerCode: Arg.Is(player.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var sut = new AVIAGamingPlayerBalanceGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameurl.Balance.ToDecimal().GetValueOrDefault(0), actual);
    }
}
