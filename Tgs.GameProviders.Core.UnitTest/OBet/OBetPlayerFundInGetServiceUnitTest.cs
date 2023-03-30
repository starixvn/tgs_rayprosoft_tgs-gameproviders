using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.OBet.Protos;
using Tgs.GameProviders.Core.OBet.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.OBet;

[TestFixture]
public class OBetPlayerFundInGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfaceOBetClient>();

        var query = new OBetPlayerFundInQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            Amount = 100M,
            OrderId = Guid.NewGuid().ToString(),
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.OBet),
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

        var configuration = new OBetConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiKey = "apiKey",
            AgentId = "agentId"
        };
        _ = fakeGameProviderRepository
            .GetOBetConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var response = new FundReply()
        {
            SerialNo = query.OrderId
        };
        _ = fakeClient
            .FundInAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                apiKey: Arg.Is(configuration.ApiKey),
                agentId: Arg.Is(configuration.AgentId),
                playerCode: Arg.Is(player.PlayerCode),
                serialNo: Arg.Is(query.OrderId),
                amount: Arg.Is(query.Amount),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

        var sut = new OBetPlayerFundInGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.SerialNo, actual.OrderId);
    }
}
