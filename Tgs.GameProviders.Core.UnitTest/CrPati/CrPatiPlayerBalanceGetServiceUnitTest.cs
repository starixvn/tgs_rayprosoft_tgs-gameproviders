using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.GameProviders.Core.CrPati.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.CrPati;

public class CrPatiPlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Normal_ProcessAsync()
    {
        var ipAddressOptions = Options.Create(new IPAddressOptions
        {
            IPAddress = "127.0.0.1"
        });
        var fakeClient = Substitute.For<IGameInterfaceCrPatiClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new CrPatiPlayerBalanceQuery
        {
            PlayerCode = "test"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.CrPati),
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
            DefaultWalletCurrencyCode: "HKD");
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

        var configuration = new CrPatiConfiguration
        {
            ApiUrl = new("http://localhost/"),
            ApiKey = "apiKey",
            ApiSecret = "apiSecret"
        };
        _ = fakeGameProvider
            .GetCrPatiConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var playerBalanceReply = new PlayerBalanceReply
        {
            IsPlayerExist = true,
            Balance = 100
        };
        _ = fakeClient
            .PlayerBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            apiKey: Arg.Is(configuration.ApiKey),
            apiSecret: Arg.Is(configuration.ApiSecret),
            playerCode: Arg.Is(player.PlayerCode),
            ipAddress: Arg.Is(ipAddressOptions.Value.IPAddress),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerBalanceReply);

        var sut = new CrPatiPlayerBalanceGetService(
            ipAddressOptions,
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut.GetAsync(query);

        Assert.Equal(100m, actual.Balance);
        Assert.Equal(100m, actual.FreeBalance);
        Assert.Equal(0m, actual.OutstandingBalance);
    }
}
