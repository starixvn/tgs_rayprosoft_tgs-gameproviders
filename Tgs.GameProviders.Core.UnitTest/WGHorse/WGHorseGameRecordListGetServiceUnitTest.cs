using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.WGHorse;
using Tgs.GameProviders.Core.WGHorse.ApplicationServices;
using Tgs.GameProviders.Core.WGHorse.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.WGHorse
{
    [TestFixture]
    public class WGHorseGameRecordListGetServiceUnitTest
    {
        [Test]
        public async Task WGHorse_GameRecord_Multiples_Is_Close()
        {
            var fakeWGHorseApiClient = Substitute.For<IWGHorseApiClient>();
            var fakeWGHorseService = Substitute.For<IWGHorseService>();

            var fakeService = new WGHorseGameRecordListGetService(fakeWGHorseApiClient, fakeWGHorseService);

            var query = new WGHorseGameRecordListQuery()
            {
                StartTime = DateTimeOffset.Parse("2022-04-16"),
                EndTime = DateTimeOffset.Parse("2022-04-17").AddMilliseconds(-1),
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
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

            var gameRound = new WGHorseGameRecord[]
            {
                new(){
                    MemberCode ="4ugl1wyedic5",
                    RecordDate ="2022-04-16 17:00:04",
                    BatchNo ="123499081",
                    ShowType= "過關<br><font color='red'>2x3</font>",
                    ShowDiscount ="有單頭@0.9000",
                    ShowContent="&nbsp;HK  9場&nbsp;@&nbsp;2022-04-16 17:10:00<br>&nbsp;@&nbsp;<font color='FireBrick'><b>QPL</b></font>&nbsp;&nbsp;<font color='FireBrick'><b>1+12</b></font>&nbsp;&nbsp;<br>&nbsp;HK  10場&nbsp;@&nbsp;2022-04-16 17:10:00<br>&nbsp;@&nbsp;<font color='FireBrick'><b>QPL</b></font>&nbsp;&nbsp;<font color='FireBrick'><b>7>12+14</b></font>&nbsp;&nbsp;<br>&nbsp;每注金額:10<Br>",
                    ShowResult="<font font-family: Arial,Verdana;>1,8,12,7,10,4,11,6,5,9,3,2</font><br><font font-family: Arial,Verdana;>8,4,3,9,14,1,5,10,2,11,7,13,12,6</font><br>",
                    ShowAmount="50",
                    ShowWin="129.00",
                    ShowOddsWin ="0",
                    ShowDivideDate= "2022-04-16 17:39:34",
                    ShowRaceDate="2022-04-16 17:10:00",
                }
            };

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                query.StartTime,
                query.EndTime,
                query.Page,
                query.Rows).Returns(ValueTask.FromResult<(bool Success, WGHorseGameRecord[]? GameRecord)>((true, gameRound)));

            _ = fakeWGHorseService.GetClientRound(decimal.Parse("129.00")).Returns(RoundClientStatus.Win);

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(true, actual.Datas.Any());
            Assert.AreEqual(RoundStatus.Closed, actual.Datas.First().Status);
            Assert.AreEqual(RoundClientStatus.Win, actual.Datas.First().ClientStatus);
            Assert.AreEqual(129.00m, actual.Datas.First().PlayerWinLoss);
            Assert.AreEqual(179, actual.Datas.First().SettleAmount);
        }

        [Test]
        public async Task WGHorse_GameRecord_Multiples_Is_Open()
        {
            var fakeWGHorseApiClient = Substitute.For<IWGHorseApiClient>();
            var fakeWGHorseService = Substitute.For<IWGHorseService>();

            var fakeService = new WGHorseGameRecordListGetService(fakeWGHorseApiClient, fakeWGHorseService);

            var query = new WGHorseGameRecordListQuery()
            {
                StartTime = DateTimeOffset.UtcNow.AddDays(-1),
                EndTime = DateTimeOffset.UtcNow,
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
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

            var gameRound = new WGHorseGameRecord[]
            {
                new(){
                    MemberCode ="4ugl1wyedic5",
                    RecordDate =DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    BatchNo ="123499081",
                    ShowType= "過關<br><font color='red'>2x3</font>",
                    ShowDiscount ="有單頭@0.9000",
                    ShowContent="&nbsp;HK  9場&nbsp;@&nbsp;2022-04-16 17:10:00<br>&nbsp;@&nbsp;<font color='FireBrick'><b>QPL</b></font>&nbsp;&nbsp;<font color='FireBrick'><b>1+12</b></font>&nbsp;&nbsp;<br>&nbsp;HK  10場&nbsp;@&nbsp;2022-04-16 17:10:00<br>&nbsp;@&nbsp;<font color='FireBrick'><b>QPL</b></font>&nbsp;&nbsp;<font color='FireBrick'><b>7>12+14</b></font>&nbsp;&nbsp;<br>&nbsp;每注金額:10<Br>",
                    ShowResult="<font font-family: Arial,Verdana;>1,8,12,7,10,4,11,6,5,9,3,2</font><br><font font-family: Arial,Verdana;>8,4,3,9,14,1,5,10,2,11,7,13,12,6</font><br>",
                    ShowAmount="50",
                    ShowWin="129.00",
                    ShowOddsWin ="0",
                    ShowDivideDate= DateTimeOffset.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    ShowRaceDate=DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                }
            };

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                query.StartTime,
                query.EndTime,
                query.Page,
                query.Rows).Returns(ValueTask.FromResult<(bool Success, WGHorseGameRecord[]? GameRecord)>((true, gameRound)));

            _ = fakeWGHorseService.GetClientRound(decimal.Parse("129.00")).Returns(RoundClientStatus.Win);

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(true, actual.Datas.Any());
            Assert.AreEqual(RoundStatus.Open, actual.Datas.First().Status);
            Assert.AreEqual(RoundClientStatus.Open, actual.Datas.First().ClientStatus);
            Assert.AreEqual(0m, actual.Datas.First().PlayerWinLoss);
            Assert.AreEqual(0m, actual.Datas.First().SettleAmount);
        }

        [Test]
        public async Task WGHorse_GameRecord_Push()
        {
            var fakeWGHorseApiClient = Substitute.For<IWGHorseApiClient>();
            var fakeWGHorseService = Substitute.For<IWGHorseService>();

            var fakeService = new WGHorseGameRecordListGetService(fakeWGHorseApiClient, fakeWGHorseService);

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
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

            var gameRound = new WGHorseGameRecord[]
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
            };

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                query.StartTime,
                query.EndTime,
                query.Page,
                query.Rows).Returns(ValueTask.FromResult<(bool Success, WGHorseGameRecord[]? GameRecord)>((true, gameRound)));

            _ = await fakeService.GetAsync(new()
            {
                StartTime = DateTimeOffset.Parse("2021-10-30"),
                EndTime = DateTimeOffset.Parse("2021-11-01").AddMilliseconds(-1),
                Page = 1,
                Rows = 1000
            }).ConfigureAwait(false);
        }

        [Test]
        public async Task WGHorse_GameRecord_Single_位置Q_Is_Close()
        {
            var fakeWGHorseApiClient = Substitute.For<IWGHorseApiClient>();
            var fakeWGHorseService = Substitute.For<IWGHorseService>();

            var fakeService = new WGHorseGameRecordListGetService(fakeWGHorseApiClient, fakeWGHorseService);

            var query = new WGHorseGameRecordListQuery()
            {
                StartTime = DateTimeOffset.UtcNow.AddDays(-1),
                EndTime = DateTimeOffset.UtcNow,
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
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

            var gameRound = new WGHorseGameRecord[]
            {
                new(){
                    MemberCode ="4ugl1wyedic5",
                    RecordDate ="2022-04-16 14:56:44",
                    BatchNo ="933761975",
                    ShowType= "位置Q",
                    ShowDiscount ="早@0.9100<br>",
                    ShowContent="HK 5場&nbsp;@&nbsp;2022-04-16 15:00:00<br>@&nbsp;<font color='FireBrick'><b>QPL 4+7 </b></font>",
                    ShowResult="6,2,5,7",
                    ShowAmount="50",
                    ShowWin="-45.50",
                    ShowOddsWin ="0.00",
                    ShowDivideDate= "2022-04-16 15:11:29",
                    ShowRaceDate="2022-04-16 15:00:00"
                }
            };

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                query.StartTime,
                query.EndTime,
                query.Page,
                query.Rows).Returns(ValueTask.FromResult<(bool Success, WGHorseGameRecord[]? GameRecord)>((true, gameRound)));

            _ = fakeWGHorseService.GetClientRound(decimal.Parse("-45.50")).Returns(RoundClientStatus.Loss);

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(true, actual.Datas.Any());
            Assert.AreEqual(RoundStatus.Closed, actual.Datas.First().Status);
            Assert.AreEqual(RoundClientStatus.Loss, actual.Datas.First().ClientStatus);
            Assert.AreEqual(-45.50m, actual.Datas.First().PlayerWinLoss);
            Assert.AreEqual(4.5m, actual.Datas.First().SettleAmount);
        }

        [Test]
        public async Task WGHorse_GameRecord_Single_獨贏_Is_Close()
        {
            var fakeWGHorseApiClient = Substitute.For<IWGHorseApiClient>();
            var fakeWGHorseService = Substitute.For<IWGHorseService>();

            var fakeService = new WGHorseGameRecordListGetService(fakeWGHorseApiClient, fakeWGHorseService);

            var query = new WGHorseGameRecordListQuery()
            {
                StartTime = DateTimeOffset.UtcNow.AddDays(-1),
                EndTime = DateTimeOffset.UtcNow,
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
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

            var gameRound = new WGHorseGameRecord[]
            {
                new(){
                    MemberCode ="4ugl1wyedic5",
                    RecordDate ="2022-04-16 14:54:53",
                    BatchNo ="833757162",
                    ShowType= "獨贏",
                    ShowDiscount ="代@0.9000<br>",
                    ShowContent="HK 5場&nbsp;@&nbsp;2022-04-16 15:00:00<br>@&nbsp;<font color='FireBrick'><b>WIN 7 </b></font>",
                    ShowResult="6,2,5,7",
                    ShowAmount="50",
                    ShowWin="-45.50",
                    ShowOddsWin ="0.00",
                    ShowDivideDate= "2022-04-16 15:11:29",
                    ShowRaceDate= "2022-04-16 15:00:00"
                }
            };

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                query.StartTime,
                query.EndTime,
                query.Page,
                query.Rows).Returns(ValueTask.FromResult<(bool Success, WGHorseGameRecord[]? GameRecord)>((true, gameRound)));

            _ = fakeWGHorseService.GetClientRound(decimal.Parse("-45.50")).Returns(RoundClientStatus.Loss);

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(true, actual.Datas.Any());
            Assert.AreEqual(RoundStatus.Closed, actual.Datas.First().Status);
            Assert.AreEqual(RoundClientStatus.Loss, actual.Datas.First().ClientStatus);
            Assert.AreEqual(-45.50m, actual.Datas.First().PlayerWinLoss);
            Assert.AreEqual(4.5m, actual.Datas.First().SettleAmount);
        }
    }
}
