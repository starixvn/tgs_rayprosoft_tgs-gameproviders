using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Common;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.GameProviders.Core.InplayMatrixESports.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.InplayMatrixESports;

[TestFixture]
public class InplayMatrixESportsPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfaceInplayMatrixClient>();

        var query = new InplayMatrixESportsPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.InplayMatrix_ESports),
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

        var configuration = new InplayMatrixESportsConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ReportUrl = new("http://localhost/"),
            MerchantCode = "apiKey",
            MerchantPrefix = "agentId",
            Password = "1q2w3e4r"
        };
        _ = fakeGameProviderRepository
            .GetInplayMatrixESportsConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var response = new PlayerBalanceReply()
        {
            IsPlayerExist = true,
            Balance = 168M
        };

        _ = fakeClient
            .PlayerBalanceAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                merchantCode: Arg.Is(configuration.MerchantCode),
                merchantPrefix: Arg.Is(configuration.MerchantPrefix),
                playerCode: Arg.Is(player.PlayerCode),
                productWallet: Arg.Is((int)InplayMatrixProductWallet.IM_eSports_Wallet),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

        var sut = new InplayMatrixESportsPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(168M, actual);
    }
}
