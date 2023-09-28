using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.EvolutionTransfer;

public class EvolutionTransferPlayerFundCheckingGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
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

        var query = new EvolutionTransferPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var createdOn = DateTimeOffset.UtcNow;

        var response = new GetTransactionInfoReply()
        {
            Result = true,
            Balance = 100M,
            TransactionId = "TransactionId",
            EvoTransactionId = "EvoTransactionId",
            CreatedOn = createdOn.UtcDateTime.ToTimestamp(),
            CreatedOnIso = createdOn.ToTimestamp(),
            PlayerCode = query.PlayerCode,
            CCid = "",
            Amount = 100m,
            ErrorMessage = ""
        };

        _ = fakeGameInterfaceEvolutionServiceGrpc.GetTransactionInfoAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.EcId),
            playerCode: query.PlayerCode,
            transactionId: Arg.Is(query.OrderId)).Returns(response);

        var sut = new EvolutionTransferPlayerFundCheckingGetService(
            fakeGameInterfaceEvolutionServiceGrpc,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.True(actual.OrderStatus);
    }
}
