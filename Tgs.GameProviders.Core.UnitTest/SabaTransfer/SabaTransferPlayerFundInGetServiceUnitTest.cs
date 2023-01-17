using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SabaTransfer;

[TestFixture]
public class SabaTransferPlayerFundInGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSabaClient = Substitute.For<IGameInterfaceSabaClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var vendorTransId = Guid.NewGuid().ToString("N");

        var query = new SabaTransferPlayerFundInQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = vendorTransId,
            Amount = 100m
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
        _ = fakeGameProviderRepository.GetSabaTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var gameProviderCurrencyCode = "20";
        _ = fakeGameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
               gameProviderId: Arg.Is(gameProviderId),
               currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(gameProviderCurrencyCode);

        var reply = new FundReply()
        {
            Check = false,
            IsOrderExist = true,
            VendorTransId = vendorTransId,
            ActualVendorTransId = $"{configuration.OperatorId}_{vendorTransId}",
            TransId = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Balance = 100,
        };
        _ = fakeGameInterfaceSabaClient
            .FundInAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                operatorId: Arg.Is(configuration.OperatorId),
                vendorId: Arg.Is(configuration.VendorId),
                vendorMemberId: Arg.Is(player.PlayerCode),
                vendorTransId: Arg.Is(query.OrderId),
                currency: Arg.Is(int.Parse(gameProviderCurrencyCode)),
                amount: Arg.Is(query.Amount))
            .Returns(reply);

        var sut = new SabaTransferPlayerFundInGetService(
            fakeGameInterfaceSabaClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
    }
}
