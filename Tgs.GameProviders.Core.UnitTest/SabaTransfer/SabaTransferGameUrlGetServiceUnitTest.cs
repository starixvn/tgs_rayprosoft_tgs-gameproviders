using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.SabaTransfer;

[TestFixture]
public class SabaTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSabaClient = Substitute.For<IGameInterfaceSabaClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var query = new SabaTransferGameUrlQuery
        {
            GameType = GameType.Slot,
            GameId = 1,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SabaTransfer)).Returns(gameProviderId);

        var player = new PlayerModel(
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

        var gameProviderCultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameProviderId),
             Arg.Is(query.Language)).Returns(gameProviderCultureCode);

        var response = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeGameInterfaceSabaClient
            .GameUrlAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                operatorId: Arg.Is(configuration.OperatorId),
                vendorId: Arg.Is(configuration.VendorId),
                vendorMemberId: Arg.Is(player.PlayerCode),
                isMobile: Arg.Is(query.PlatformType == PlatformType.Mobile))
            .Returns(response);

        var sut = new SabaTransferGameUrlGetService(
            fakeGameInterfaceSabaClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri("http://localhost/"), actual);
    }
}
