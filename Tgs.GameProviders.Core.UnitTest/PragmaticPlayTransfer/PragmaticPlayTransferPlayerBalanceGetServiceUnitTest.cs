using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

[TestFixture]
public class PragmaticPlayTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task GetBalance_When_Player_Not_Exist()
    {
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();
        var fakeGamePlayer = Substitute.For<IPragmaticPlayTransferGamePlayer>();
        var fakePlayerService = Substitute.For<IPlayerRepository>();

        var configuration = new PragmaticPlayTransferConfiguration()
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var gameProviderId = Guid.Parse("e4ecfa98-b373-4123-87e0-cb9e883d0691");
        var gameProviderCurrencyCode = "CNY";

        var player = new PlayerModel(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var balance = 100m;

        var query = new PragmaticPlayTransferPlayerBalanceQuery
        {
            PlayerCode = player.PlayerCode
        };

        _ = fakePlayerService.GetAsync(Arg.Is(player.PlayerCode)).Returns(player);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeApiClient
            .GetBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is(player.PlayerCode))
            .Returns(
                x => (false, null),
                x => (true, 100m));

        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeService
            .GetGameProviderCurrencyCodeAsync(
            Arg.Is(gameProviderId),
            Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(gameProviderCurrencyCode);

        _ = fakeGamePlayer
            .CreatePlayerAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProviderCurrencyCode),
                Arg.Is(configuration))
            .Returns(true);

        var sut = new PragmaticPlayTransferPlayerBalanceGetService(
            fakePlayerService,
            fakeService,
            fakeGamePlayer,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.AreEqual(balance, actual);

        _ = fakeGamePlayer
            .Received(1)
            .CreatePlayerAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProviderCurrencyCode),
                Arg.Is(configuration));

        _ = fakeApiClient
            .Received(2)
            .GetBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is(player.PlayerCode));
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();
        var fakeGamePlayer = Substitute.For<IPragmaticPlayTransferGamePlayer>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var configuration = new PragmaticPlayTransferConfiguration
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var gameProviderCurrencyCode = "CNY";

        var player = new PlayerModel(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var balance = 100m;

        var query = new PragmaticPlayTransferPlayerBalanceQuery
        {
            PlayerCode = player.PlayerCode
        };

        _ = fakePlayer.GetAsync(Arg.Is(player.PlayerCode)).Returns(player);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeApiClient
            .GetBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is(player.PlayerCode))
            .Returns((true, balance));

        var sut = new PragmaticPlayTransferPlayerBalanceGetService(
            fakePlayer,
            fakeService,
            fakeGamePlayer,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.AreEqual(balance, actual);

        _ = fakeGamePlayer
            .DidNotReceive()
            .CreatePlayerAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProviderCurrencyCode),
                Arg.Is(configuration));
    }
}
