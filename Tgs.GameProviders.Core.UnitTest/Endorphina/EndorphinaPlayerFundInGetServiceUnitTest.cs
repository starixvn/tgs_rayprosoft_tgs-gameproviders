using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EndorphinaService.Protos;
using Tgs.GameProviders.Core.Endorphina.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Endorphina;

public class EndorphinaPlayerFundInGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeGameInterfaceEndorphinaClient = Substitute.For<IGameInterfaceEndorphinaClient>();

        var query = new EndorphinaPlayerFundInQuery
        {
            PlayerCode = "4ucmqplxb029",
            Amount = 100M,
            OrderId = Guid.NewGuid().ToString(),
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.Endorphina)).Returns(gameProviderId);

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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new EndorphinaConfiguration()
        {
            ApiUrl = new("https://test.endorphina.network"),
            MerchantId = "1494",
            SecretKey = "9BC9E97FFCD74B16B87BD6FA82C8C2AF",
        };
        _ = fakeGameProviderRepository
            .GetEndorphinaConfigurationAsync(
                Arg.Is(gameProviderConfigurationId),
                Arg.Any<CancellationToken>())
            .Returns(configuration);

        _ = fakeGameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
                Arg.Is(gameProviderId),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Any<CancellationToken>())
            .Returns("CNY");

        var response = new FundReply()
        {
            Balance = 168M,
            RefTransactionCode = query.OrderId,
            TransactionId = "2263122",
        };

        _ = fakeGameInterfaceEndorphinaClient
            .FundInAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.MerchantId),
                Arg.Is(configuration.SecretKey),
                Arg.Is(player.PlayerCode),
                Arg.Is("CNY"),
                Arg.Is(query.OrderId),
                Arg.Is(query.Amount),
                Arg.Any<CancellationToken>())
            .Returns(response);

        var sut = new EndorphinaPlayerFundInGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceEndorphinaClient,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(response.TransactionId, actual.OrderId);
    }
}
