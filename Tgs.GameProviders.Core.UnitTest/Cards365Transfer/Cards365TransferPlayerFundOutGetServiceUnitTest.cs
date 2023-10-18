using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameInterface.Cards365.Protos;
using Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Cards365Transfer;

public class Cards365TransferPlayerFundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfaceCards365Client>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();

        var query = new Cards365TransferPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            Amount = 100M,
            OrderId = Guid.Parse("7f5a44a8-74bf-4927-8152-b9482a42af9c").ToString("N"),
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.Cards365Transfer),
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

        var configuration = new Cards365TransferConfiguration()
        {
            ApiUrl = new("http://localhost"),
            Website = "website",
            Cert = "cert",
            Extension1 = "extension1"
        };
        _ = fakeGameProviderRepository
            .GetCards365TransferConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        _ = fakeSnowflakeWork.GetId()
            .Returns(1714154181705543700);

        var orderId = "D1714154181705543700";
        var reply = new FundReply()
        {
            TransactionId = orderId,
            Resp = new JsonObject { ["status"] = "1" }.ToJsonString()
        };

        _ = fakeClient.FundOutAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            website: Arg.Is(configuration.Website),
            cert: Arg.Is(configuration.Cert),
            extension1: Arg.Is(configuration.Extension1),
            playerCode: Arg.Is(player.PlayerCode),
            transactionId: Arg.Is(orderId),
            amount: Arg.Is(query.Amount),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(reply);

        var sut = new Cards365TransferPlayerFundOutGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeSnowflakeWork,
            fakeClient);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(reply.TransactionId, actual.OrderId);
    }
}
