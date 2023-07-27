using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcAelotto;

public class AwcAelottoPlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Create_Player()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcAelottoPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_AELOTTO),
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

        var configuration = new AwcAelottoConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcAelottoConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var response = new BalanceReply()
        {
            Success = true,
            IsPlayerExist = false,
            Balance = 0.0m
        };
        _ = fakeClient.GetBalanceAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.AgentId),
            Arg.Is(configuration.Cert),
            Arg.Is(query.PlayerCode)).Returns(response);

        var currency = "CNY";
        _ = fakeGameProvider
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currency);

        var sut = new AwcAelottoPlayerBalanceGetService(
            fakeBrand,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        _ = await sut.GetAsync(query: query).ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .CreatePlayerAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.AgentId),
            cert: Arg.Is(configuration.Cert),
            playerCode: Arg.Is(query.PlayerCode),
            userName: Arg.Is(query.PlayerCode),
            currency: Arg.Is(currency),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var query = new AwcAelottoPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_AELOTTO),
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

        var configuration = new AwcAelottoConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider
            .GetAwcAelottoConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var response = new BalanceReply()
        {
            Success = true,
            IsPlayerExist = true,
            Balance = 100m
        };
        _ = fakeClient.GetBalanceAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.AgentId),
            Arg.Is(configuration.Cert),
            Arg.Is(query.PlayerCode)).Returns(response);

        var sut = new AwcAelottoPlayerBalanceGetService(
            fakeBrand,
            fakeGameProvider,
            fakeClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(response.Balance.ToDecimal(), actual.Balance);
    }
}
