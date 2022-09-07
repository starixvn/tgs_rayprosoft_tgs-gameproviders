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
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

[TestFixture]
public class WMCasinoTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<WMCasinoTransferCreatePlayerCommand>>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var gameUrl = new Uri("http://www.google.com");

        var configuration = new WMCasinoTransferConfiguration
        {
            ApiUrl = new Uri("https://api.a45.me/api/public/Gateway.php")
        };

        var configurationData = new WMCasinoTransferConfigurationData(
            VendorId: "testVendorId",
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

        var gameModel = new GameModel(
            1,
            Guid.NewGuid(),
            GameProviderType.WMCasinoTransfer,
            "test",
            GameStatus.Active,
            null,
            null,
            null,
            GameType.Slot,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "test",
            "test",
            null,
            GameProviderType.WMCasinoTransfer);

        var query = new WMCasinoTransferGameUrlQuery
        {
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "0",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };
        var gameProviderCultureCode = "0";
        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameModel.GameProviderId);
        _ = fakeSettingRepository.GetWMCasinoTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameModel.GameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
            Arg.Is(gameModel.GameProviderId),
            Arg.Is(query.Language)).Returns(gameProviderCultureCode);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameModel);

        var response = new GameInterfaceWMCasinoServiceGameUrlReply()
        {
            Success = true,
            IsPlayerExist = true,
            GameUrl = gameUrl.ToString()
        };

        var user = $"wmtf_{playerModel.PlayerCode}_{playerModel.BrandCode}";

        _ = fakeGameInterfaceWMCasinoServiceGrpc.GameUrlAsync(
             apiUrl: Arg.Is(configuration.ApiUrl),
             vendorId: Arg.Is(configurationData.VendorId),
             signature: Arg.Is(configurationData.Signature),
             user: Arg.Is(user),
             password: Arg.Any<string>(),
             language: Arg.Any<int>()
            ).Returns(response);

        var sut = new WMCasinoTransferGameUrlGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameProviderRepository,
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
