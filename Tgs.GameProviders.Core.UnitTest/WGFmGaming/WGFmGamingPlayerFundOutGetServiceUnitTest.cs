using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

[TestFixture]
public class WGFmGamingPlayerFundOutGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();

        var query = new WGFmGamingPlayerFundOutQuery
        {
            BrandCode = "BrandCode",
            CurrencyCode = "CNY",
            PlayerCode = "test",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 1
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WGSports)).Returns(ValueTask.FromResult(gameProviderId));

        var player = new PlayerModel(
                   ResellerId: Guid.NewGuid(),
                   ResellerCode: "GPI",
                   LicenseeId: Guid.NewGuid(),
                   LicenseeCode: "GPI",
                   BrandId: Guid.NewGuid(),
                   BrandCode: "GPI",
                   PlayerCode: "test",
                   Username: "test",
                   DefaultWalletId: Guid.NewGuid(),
                   DefaultWalletCurrencyCode: "CNY",
                   PlayerStatus.Enabled);
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
            Arg.Is(gameProviderId),
            Arg.Is(player.BrandId)).Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configuration = new WGFmGamingConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };
        _ = fakeGameProviderRepository.GetWGFmGamingConfigurationData(
            Arg.Is(gameProviderConfigurationId)
            ).Returns(ValueTask.FromResult(configuration));

        _ = fakeGameInterfaceWGFmGamingClient
            .FundOutAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.PartnerCode),
                Arg.Is(configuration.PartnerKey),
                Arg.Is(configuration.IV),
                Arg.Is(query.PlayerCode),
                Arg.Is(query.OrderId),
                Arg.Is(query.Amount));

        var sut = new WGFmGamingPlayerFundOutGetService(
            fakeGameInterfaceWGFmGamingClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(query.OrderId, actual.OrderId);
    }
}
