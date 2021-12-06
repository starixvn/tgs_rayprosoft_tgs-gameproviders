using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.Core.WGSports.Models;
using TGS.GameProviders.Core.WGSports.Models.Commons;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.WGSports
{
    [TestFixture]
    public class WGSportsGameRecordListGetServiceUnitTest
    {
        [Test]
        public async Task WGSports_GameRecord_List()
        {
            var faceLogger = NullLogger<WGSportsGameRecordListGetService>.Instance;
            var fakeWGHorseApiClient = Substitute.For<IWGSportsApiClient>();
            var fakeWGHorseService = Substitute.For<IWGSportsService>();

            var fakeService = new WGSportsGameRecordListGetService(fakeWGHorseApiClient, faceLogger, fakeWGHorseService);

            var query = new WGSportsGameRecordListQuery()
            {
                IsSettlement = true,
                StartTime = DateTimeOffset.Parse("2021-12-04 15:00:00"),
                EndTime = DateTimeOffset.Parse("2021-12-04 15:20:00")
            };

            var configuration = new WGSportsConfiguration()
            {
                AmountMaximum = 999999999999,
                AmountFormat = "12,2",
                ApiUrl = new("https://api.jsc080.net/"),
                PartnerCode = "TA",
                PartnerKey = "abc1238888888888",
                IV = "cba1238888888888",
                ServiceEndpointUrl = new("http://game-wgvia")
            };

            var gameRecordResponse = new WGSportsGameRecordResponse()
            {
                Total = "1",
                TotalAmount = "0",
                TotalResult = "0",
                TotalActiveAmount = "0",
                ReturnCode = "1",
                BetInfo = new WGSportsBetInfo[]
                    {
                        new(){
                            MemberId ="29509",
                            Username= "4ueycx9cqj55",
                            RecordDate ="2021-12-04 10:39:53",
                            Reference = "5120488483",
                            SportType ="足球",
                            BetType ="滚球大/小",
                            MatchName ="阿拉伯杯2021(在卡塔尔)",
                            HomeName ="黎巴嫩",
                            AwayName ="阿尔及利亚",
                            BetContent = "阿拉伯杯2021(在卡塔尔)<br>黎巴嫩  VS  阿尔及利亚 (0:2)<br>0:1 1.5 小盘 @&nbsp; 0.460",
                            BetAmount ="<s>111.00</s>",
                            BetStatus = "<font color='red'>比分错误</font>",
                            Result = "<font color='red'>比分错误</font>",
                            ActiveAmount ="<s>111.00</s>",
                            ResultScore ="0:0/0:2",
                            SettleDate="2021-12-04 11:06:28",
                            BetIp ="218.250.125.94",
                            InitData =new()
                            {
                                 FB = "1",
                                Type ="live_highlow",
                                Content ="low@0.46@1.50@0:1",
                                Valid ="-5",
                                LiveStatus ="1",
                                PlanNo ="1",
                                Part ="0",
                                Win ="1",
                                OddsType="1",
                                MatchDate ="2021-12-04 09:00:00",
                                BetAmount ="111.00"
                            }}
                    }
            };
            _ = fakeWGHorseService.GetSettingConfigurationAsync().Returns(Task.FromResult(configuration));
            _ = fakeWGHorseService.GetClientRound(gameRecordResponse.BetInfo[0].Result).Returns(ClientRoundStatus.Void);

            _ = fakeWGHorseApiClient.GetGameRecordAsync(
                 configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                new()
                {
                    GameRecordTime = GameRecordTime.SettleDate,
                    StartTime = query.StartTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = query.EndTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss")
                }).Returns(ValueTask.FromResult(gameRecordResponse));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);
            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(ClientRoundStatus.Void, actual.Datas[0].ClientStatus);
        }
    }
}
