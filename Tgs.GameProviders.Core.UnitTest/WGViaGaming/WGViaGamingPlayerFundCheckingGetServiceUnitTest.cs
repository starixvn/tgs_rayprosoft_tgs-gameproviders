using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameProviders.Core.WGViaGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGViaGaming;

public class WGViaGamingPlayerFundCheckingGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceWGViaGamingClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();

        var query = new WGViaGamingPlayerFundCheckingQuery
        {
            BrandCode = "brandcode",
            PlayerCode = "test",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.WGViaGaming),
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
        _ = fakePlayerRepository
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configurationData = new WGViaGamingConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };
        _ = fakeGameProviderRepository
            .GetWGViaGamingConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configurationData));

        var sut = new WGViaGamingPlayerFundCheckingGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        _ = await sut
           .GetAsync(
            query: query)
           .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .FundStatusAsync(
            apiUrl: Arg.Is(configurationData.ApiUrl),
            partnerCode: Arg.Is(configurationData.PartnerCode),
            secretKey: Arg.Is(configurationData.PartnerKey),
            secretIV: Arg.Is(configurationData.IV),
            orderId: Arg.Is(query.OrderId),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
