using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.EvolutionTransfer;

public class EvolutionTransferPlayerFundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new EvolutionTransferConfiguration
        {
            Ua2AuthenticationUrl = new("https://babylonpega2.uat1.evo-test.com/ua/v1/babylonpega20001/test123"),
            ApiUrl = new("https://babylonpega2.uat1.evo-test.com"),
            CasinoId = "babylonpega20001",
            Ua2Token = "test123",
            GameHistoryApiToken = "test123",
            ExternalLobbyApiToken = "test123",
            EcId = "babylonpega20001test123"
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

        var query = new EvolutionTransferPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
        };

        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

        var reply = new FundOutReply()
        {
            Result = true,
            TransactionId = query.OrderId,
            EvoTransactionId = "EvoTransactionId",
            PlayerCode = query.PlayerCode,
            Uid = "Uid",
            Balance = 100M,
            ErrorMessage = ""
        };
        _ = fakeGameInterfaceEvolutionServiceGrpc.FundOutAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.EcId),
            playerCode: Arg.Is(query.PlayerCode),
            transactionId: Arg.Is(query.OrderId),
            amount: Arg.Is(query.Amount)).Returns(reply);

        var sut = new EvolutionTransferPlayerFundOutGetService(
            fakeGameInterfaceEvolutionServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(query.OrderId, actual.OrderId);
    }
}
