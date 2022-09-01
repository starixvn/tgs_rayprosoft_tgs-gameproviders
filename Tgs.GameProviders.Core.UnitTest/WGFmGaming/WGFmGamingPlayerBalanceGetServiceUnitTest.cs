using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

[TestFixture]
public class WGFmGamingPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new WGFmGamingPlayerBalanceQuery
        {
            PlayerCode = "test"
        };

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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

        var configuration = new WGSportsConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };
        _ = fakeSettingRepository.GetWGSportsConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        (bool IsPlayer, decimal balance) = (true, 1);
        _ = fakeGameInterfaceWGFmGamingClient.PlayerBalanceAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.PartnerCode),
            Arg.Is(configuration.PartnerKey),
            Arg.Is(configuration.IV),
            Arg.Is(playerModel.PlayerCode)).Returns((IsPlayer, balance));

        var sut = new WGFmGamingPlayerBalanceGetService(
             fakeGameInterfaceWGFmGamingClient,
             fakeSettingRepository,
             fakePlayerRepository,
             fakeGameProviderRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(balance, actual.Value);
    }
}
