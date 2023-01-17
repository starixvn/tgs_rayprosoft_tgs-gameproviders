using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

[TestFixture]
public class WMCasinoTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<WMCasinoTransferCreatePlayerCommand>>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();
        var fakeLogger = NullLogger<WMCasinoTransferPlayerBalanceGetService>.Instance;

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

        var query = new WMCasinoTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameProviderId);
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var response = new GameInterfaceWMCasinoServiceBalanceReply()
        {
            Success = true,
            IsPlayerExist = true,
            Balance = 100m
        };

        var user = $"wmtf_{query.PlayerCode}_{playerModel.BrandCode}";

        _ = fakeGameInterfaceWMCasinoServiceGrpc.GetBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            vendorId: Arg.Is(configuration.VendorId),
            signature: Arg.Is(configuration.Signature),
            user: Arg.Is(user)).Returns(response);

        var sut = new WMCasinoTransferPlayerBalanceGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameProviderRepository,
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository,
            fakeLogger);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.Balance.ToDecimal(), actual.Value);
    }
}
