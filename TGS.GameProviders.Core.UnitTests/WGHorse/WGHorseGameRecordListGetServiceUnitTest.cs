using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.Core.WGHorse.Models;
using TGS.GameProviders.Core.WGHorse.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.WGHorse
{
    [TestFixture]
    public class WGHorseGameRecordListGetServiceUnitTest
    {
        [Test]
        public async Task WGHorse_GameRecord_List()
        {
            var faceLogger = NullLogger<WGHorseGameRecordListGetService>.Instance;
            var fakeWGHorseApiClient = Substitute.For<IWGHorseApiClient>();
            var fakeWGHorseService = Substitute.For<IWGHorseService>();

            var fakeService = new WGHorseGameRecordListGetService(fakeWGHorseApiClient, faceLogger, fakeWGHorseService);

            var query = new WGHorseGameRecordListQuery()
            {
                StartTime = DateTimeOffset.Parse("2021-10-30"),
                EndTime = DateTimeOffset.Parse("2021-11-01").AddMilliseconds(-1),
                Page = 1,
                Rows = 1000
            };

            var configuration = new WGHorseConfiguration()
            {
                AmountMaximum = 999999999999,
                AmountFormat = "12,2",
                ApiUrl = new("http://61.56.84.139/via6/index.php"),
                PartnerCode = "PT",
                PartnerKey = "a888888888888888",
                IV = "b888888888888888",
                ServiceEndpointUrl = new("http://game-wgvia")
            };
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(Task.FromResult(configuration));

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                new()
                {
                    StartTime = query.StartTime,
                    EndTime = query.EndTime,
                    PageIndex = query.Page,
                    PageSize = query.Rows
                }).Returns(ValueTask.FromResult(new WGHorseGameRecordResponse()
                {
                    ReturnCode = 1,
                    Page = "1",
                    PageTotal = "1",
                    PageSize = "1000",
                    RaceDateFrom = "2021-10-30",
                    RaceDateTo = "2021-10-31",
                    ArrayInvoice = new WGHorseGameRecord[]
                    {
                        new(){
                            MemberCode ="4uf2njegpusz",
                            RecordDate ="2021-10-31 13:04:19",
                            BatchNo ="123499081",
                            ShowType= "獨贏+位置",
                            ShowDiscount ="早@0.8300<br>早@0.8700<br>",
                            ShowContent="HK 2場&nbsp;@&nbsp;2021-10-31 13:15:00<br>@&nbsp;<font color='FireBrick'><b>WP</b></font>&nbsp;&nbsp;<font color='FireBrick'><B>1</font></B></s>&nbsp;&nbsp;<br>每注金額:200<Br>",
                            ShowResult="7,1,4,5,2,6,3",
                            ShowAmount="400",
                            ShowWin="0.00<br>和局",
                            ShowOddsWin ="0",
                            ShowDivideDate= "2021-10-31 13:22:25",
                            ShowRaceDate="2021-10-31 13:15:00"
                        }
                    }
                }));

            await fakeService.GetAsync(new()
            {
                StartTime = DateTimeOffset.Parse("2021-10-30"),
                EndTime = DateTimeOffset.Parse("2021-11-01").AddMilliseconds(-1),
                Page = 1,
                Rows = 1000
            }).ConfigureAwait(false);
        }
    }
}
