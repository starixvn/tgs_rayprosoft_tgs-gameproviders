using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SabaTransfer;

public class SabaTransferPlayerFundCheckingGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSabaClient = Substitute.For<IGameInterfaceSabaClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var query = new SabaTransferPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SabaTransfer)).Returns(gameProviderId);

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

        var configuration = new SabaTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            OperatorId = "tg",
            VendorId = "um5vspnzkx",
            OddsType = 2
        };
        _ = fakeGameProviderRepository.GetSabaTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        _ = fakeGameInterfaceSabaClient
                .FundStatusAsync(
                    apiUrl: Arg.Is(configuration.ApiUrl),
                    operatorId: Arg.Is(configuration.OperatorId),
                    vendorId: Arg.Is(configuration.VendorId),
                    vendorTransId: Arg.Is(query.OrderId))
                .Returns(new FundStatusReply()
                {
                    Retry = false,
                    IsOrderExist = true
                });

        var sut = new SabaTransferPlayerFundCheckingGetService(
            fakeGameInterfaceSabaClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        var actual = await sut.GetAsync(query);

        Assert.True(actual);
    }
}
