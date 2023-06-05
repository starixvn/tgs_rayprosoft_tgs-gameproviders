using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests.GameRecordList;

[TestFixture]
public class JokerGameRecordListUnitTest
{
    [Test]
    public async Task Test_Joker_GameRecordListAsync()
    {
        var query = new JokerGameRecordListRequest
        {
            BrandCode = "Default",
            AgentId = "TGVY",
            StartTime = DateTimeOffset.Parse("2021-04-20T00:00:00+00:00"),
            EndTime = DateTimeOffset.Parse("2021-04-21T00:00:00+00:00"),
            NextId = null
        };

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<JokerGameRecordListQuery, JokerGameRecordResult>>();

                var result = new JokerGameRecordResult
                {
                    Success = true,
                    NextId = "eyIyMDIyMDQyMDA5NTgiOnsiQ3JlYXRlZFRpbWUiOiI1NTdmOGNjMC1jMDRkLTExZWMtYmVlYy1kZjk4OWNhYjE5MGMifX0=",
                    Datas = new GameRecordListModel[]
                    {
                    new GameRecordListModel
                    {
                        GameProviderCode=GameProviderType.Joker,
                        GameProviderId=Guid.NewGuid(),
                        ExternalRoundId = "xmkeajuna89ne",
                        ExternalGameId= "7rw3tfwz11kaw",
                        PlayerCode = "4UF6K014JP5X",
                        Status=GameRounds.Common.GameRoundStatus.Closed,
                        ClientStatus=GameRounds.Common.GameRoundClientStatus.Win,
                        BetAmount = 1.60m,
                        ExternalCreatedOn=DateTimeOffset.Parse("2022-04-20T09:58:08.9660000+08:00"),
                        ExternalClosedOn=DateTimeOffset.Parse("2022-04-20T09:58:08.9660000+08:00"),
                        ValidBetAmount = 1.60m,
                        SettleAmount= 3.20m,
                        Commission=0,
                        Turnover=1.6m,
                        PlayerWinLoss=1.6m,
                        TieBetAdjustment=null,
                        AdditionalData=JsonSerializer.Serialize(new{ TransactionId = "xmkeajuna89ne" })
                    }
                    }
                };

                _ = fakeService
                .GetAsync(
                        query: Arg.Any<JokerGameRecordListQuery>(),
                        cancellationToken: Arg.Any<CancellationToken>())
                .Returns(result);

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameRecordListClient(httpClient);

        var actual = await sut
            .JokerGameRecordListAsync(
                query.BrandCode,
                query.AgentId,
                query.StartTime,
                query.EndTime,
                query.NextId)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.True(actual!.Success);
        Assert.True(actual.Datas!.Any());
    }
}
