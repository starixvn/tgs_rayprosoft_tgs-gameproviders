using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.PragmaticPlay.Protos;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

[TestFixture]
public class PragmaticPlayTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public void Create_Player()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

        var query = new PragmaticPlayTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.PragmaticPlayTransfer),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

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
        _ = fakePlayerRepository
            .GetAsync(
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
                 gameProviderId: Arg.Is(gameProviderId),
                 brandId: Arg.Is(player.BrandId),
                 cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new PragmaticPlayTransferConfiguration()
        {
            ApiUrl = new("https://localhost/"),
            SecureLogin = "secure_login",
            SecretKey = "secret_key"
        };
        _ = fakeGameProviderRepository
            .GetPragmaticPlayTransferConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var playerBalanceReply = new PlayerBalanceReply
        {
            IsPlayerExist = false,
            Balance = 0.0m
        };
        _ = fakeClient
            .PlayerBalanceAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                secureLogin: Arg.Is(configuration.SecureLogin),
                secretKey: Arg.Is(configuration.SecretKey),
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerBalanceReply);

        var currencyCode = "CNY";
        _ = fakeGameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currencyCode);

        var sut = new PragmaticPlayTransferPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        _ = Assert.ThrowsAsync<Exception>(() => sut.GetAsync(query).AsTask());

        _ = fakeClient
            .Received(1)
            .PlayerCreateAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                secureLogin: Arg.Is(configuration.SecureLogin),
                secretKey: Arg.Is(configuration.SecretKey),
                playerCode: Arg.Is(query.PlayerCode),
                currencyCode: Arg.Is(currencyCode),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

        var query = new PragmaticPlayTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.PragmaticPlayTransfer),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

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
        _ = fakePlayerRepository
            .GetAsync(
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
                 gameProviderId: Arg.Is(gameProviderId),
                 brandId: Arg.Is(player.BrandId),
                 cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new PragmaticPlayTransferConfiguration()
        {
            ApiUrl = new("https://localhost/"),
            SecureLogin = "secure_login",
            SecretKey = "secret_key"
        };
        _ = fakeGameProviderRepository
            .GetPragmaticPlayTransferConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var playerBalanceReply = new PlayerBalanceReply
        {
            IsPlayerExist = true,
            Balance = 100m
        };
        _ = fakeClient
            .PlayerBalanceAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                secureLogin: Arg.Is(configuration.SecureLogin),
                secretKey: Arg.Is(configuration.SecretKey),
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerBalanceReply);

        var sut = new PragmaticPlayTransferPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        var actual = await sut
            .GetAsync(
                query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(playerBalanceReply.Balance, actual);
    }
}
