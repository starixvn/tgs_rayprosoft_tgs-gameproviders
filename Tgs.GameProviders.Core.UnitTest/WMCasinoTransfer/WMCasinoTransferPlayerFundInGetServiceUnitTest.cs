using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

public class WMCasinoTransferPlayerFundInGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceWMCasinoServiceClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var configuration = new WMCasinoTransferConfiguration
        {
            ApiUrl = new Uri("https://api.a45.me/api/public/Gateway.php"),
            VendorId = "testVendorId",
            Signature = "testsignature"
        };

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

        var query = new WMCasinoTransferPlayerFundInQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameProviderId);
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var gameProviderTransactionId = "2147649635";
        var reply = new FundReply()
        {
            Success = true,
            TransactionId = query.OrderId,
            GameProviderTransactionId = gameProviderTransactionId,
            CurrentBalance = 168m
        };

        var user = $"wmtf_{playerModel.PlayerCode}_{playerModel.BrandCode}";

        _ = fakeClient.FundInAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            vendorId: Arg.Is(configuration.VendorId),
            signature: Arg.Is(configuration.Signature),
            user: Arg.Is(user),
            transactionId: Arg.Is(query.OrderId),
            amount: Arg.Is(query.Amount)).Returns(reply);

        var sut = new WMCasinoTransferPlayerFundInGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeClient,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameProviderTransactionId, actual.OrderId);
    }
}
