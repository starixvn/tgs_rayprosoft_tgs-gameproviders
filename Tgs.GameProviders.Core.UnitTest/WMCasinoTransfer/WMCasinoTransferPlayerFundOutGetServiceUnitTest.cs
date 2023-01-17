using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

[TestFixture]
public class WMCasinoTransferPlayerFundOutGetServiceUnitTest
{
    [Test]
    public void Can_Not_Fund_Out_Over_WM_Agent_Balance()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

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

        var query = new WMCasinoTransferPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 123456m
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameProviderId);
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var agentBalanceReply = new GameInterfaceWMCasinoServiceAgentBalanceReply
        {
            Balance = 1000m,
            Success = true,
            Message = "操作成功"
        };

        _ = fakeGameInterfaceWMCasinoServiceGrpc.GetAgentBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            vendorId: Arg.Is(configuration.VendorId),
            signature: Arg.Is(configuration.Signature)).Returns(agentBalanceReply);

        var sut = new WMCasinoTransferPlayerFundOutGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        Assert.ThrowsAsync<Exception>(async () => await sut.GetAsync(query).ConfigureAwait(false));
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

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

        var query = new WMCasinoTransferPlayerFundOutQuery
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
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var gameProviderTransactionId = "2147647425";
        var agentBalanceReply = new GameInterfaceWMCasinoServiceAgentBalanceReply
        {
            Balance = 100000m,
            Success = true,
            Message = "操作成功"
        };

        _ = fakeGameInterfaceWMCasinoServiceGrpc.GetAgentBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            vendorId: Arg.Is(configuration.VendorId),
            signature: Arg.Is(configuration.Signature)).Returns(agentBalanceReply);

        var reply = new GameInterfaceWMCasinoServiceFundReply()
        {
            Success = true,
            TransactionId = query.OrderId,
            GameProviderTransactionId = gameProviderTransactionId,
            CurrentBalance = 168m
        };

        var user = $"wmtf_{playerModel.PlayerCode}_{playerModel.BrandCode}";
        _ = fakeGameInterfaceWMCasinoServiceGrpc.FundOutAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            vendorId: Arg.Is(configuration.VendorId),
            signature: Arg.Is(configuration.Signature),
            user: Arg.Is(user),
            transactionId: Arg.Is(query.OrderId),
            amount: Arg.Is(query.Amount)).Returns(reply);

        var sut = new WMCasinoTransferPlayerFundOutGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameProviderTransactionId, actual.OrderId);
    }
}
