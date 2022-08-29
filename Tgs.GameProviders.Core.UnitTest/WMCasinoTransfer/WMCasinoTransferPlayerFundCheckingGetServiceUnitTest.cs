using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

[TestFixture]
public class WMCasinoTransferPlayerFundCheckingGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new WMCasinoTransferConfiguration
        {
            ApiUrl = new Uri("https://api.a45.me/api/public/Gateway.php")
        };

        var configurationData = new WMCasinoTransferConfigurationData(
            VendorId: "testAgentId",
            Signature: "testsignature");

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);

        var query = new WMCasinoTransferPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetWMCasinoTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);

        var reply = new GameInterfaceWMCasinoServiceFundOrderReply { };
        reply.Items.Add(new GameInterfaceWMCasinoServiceFundOrder
        {
            User = "user"
        });

        _ = fakeGameInterfaceWMCasinoServiceGrpc.FundOrderAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            vendorId: Arg.Is(configurationData.VendorId),
            signature: Arg.Is(configurationData.Signature),
            startTime: Arg.Any<DateTime?>(),
            endTime: Arg.Any<DateTime?>(),
            user: Arg.Any<string?>(),
            transactionId: Arg.Is(query.OrderId),
            gameProviderTransactionId: Arg.Any<string?>()).Returns(reply);

        var sut = new WMCasinoTransferPlayerFundCheckingGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(true, actual.OrderStatus);
    }
}
