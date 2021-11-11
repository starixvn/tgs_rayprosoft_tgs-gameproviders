using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WGHorse;
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
                ApiUrl = new("https://swapi.jsc080.net/index.php"),
                PartnerCode = "GP",
                PartnerKey = "gp#899la*am&m39!",
                IV = "gp01&ua!46L*ks0k",
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
                            RecordDate ="2021-10-31 17:53:13",
                            BatchNo ="123693122",
                            ShowType= "獨贏+位置",
                            ShowDiscount ="早@0.8700<br>早@0.8900<br>",
                            ShowContent="HK 10場&nbsp;@&nbsp;2021-10-31 17:55:00<br>@&nbsp;<font color='FireBrick'><b>WP</b></font>&nbsp;&nbsp;<font color='FireBrick'><B>2</font></B></s>&nbsp;&nbsp;<br>每注金額:300<Br>",
                            ShowResult="5,7,3,4,10,8,6,12,2,1,11,9",
                            ShowAmount="600",
                            ShowWin= "-528.00",
                            ShowOddsWin ="0",
                            ShowDivideDate= "2021-10-31 18:09:35",
                            ShowRaceDate="2021-10-31 17:55:00"
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
