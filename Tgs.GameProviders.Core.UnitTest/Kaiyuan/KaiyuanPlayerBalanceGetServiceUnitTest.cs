using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameInterface.Kaiyuan.Protos;
using Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Kaiyuan;

public class KaiyuanPlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Normal_ProcessAsync()
    {
        var fakeClient = Substitute.For<IGameInterfaceKaiyuanClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new KaiyuanPlayerBalanceQuery
        {
            PlayerCode = "test"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.Kaiyuan),
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

        var configuration = new KaiyuanConfiguration
        {
            AmountMaximum = 999999999999,
            AmountFormat = "12,2",
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
            Deskey = "deskey",
            Md5key = "md5key",
            CurrencyCode = "HKD",
            AgentId = "200995"
        };
        _ = fakeGameProvider
            .GetKaiyuanConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var playerBalanceReply = new PlayerBalanceReply
        {
            LockBalance = 1,
            FreeBalance = 99,
            Balance = 100
        };
        _ = fakeClient
            .PlayerBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            deskey: Arg.Is(configuration.Deskey),
            md5key: Arg.Is(configuration.Md5key),
            agentId: Arg.Is(configuration.AgentId),
            playerCode: Arg.Is(player.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerBalanceReply);

        var sut = new KaiyuanPlayerBalanceGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.Equal(100m, actual.Balance);
        Assert.Equal(99m, actual.FreeBalance);
        Assert.Equal(1m, actual.OccupyBalance);
    }
}
