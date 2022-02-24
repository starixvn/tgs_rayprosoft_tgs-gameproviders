using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.Avia.ApplicationServices;
using TGS.GameProviders.Core.Avia.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Core.UnitTests.Avia
{
    [TestFixture]
    public class AviaGameRecordListGetServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<AviaGameRecordListGetService>.Instance;
            var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
            var fakeAviaService = Substitute.For<IAviaService>();

            var fakeService = new AviaGameRecordListGetService(fakeAviaApiClient, faceLogger, fakeAviaService);

            var query = new AviaGameRecordListQuery()
            {
                BrandCode = "Default",
                StartTime = DateTimeOffset.Parse("2021-12-01T00:00:00+00:00"),
                EndTime = DateTimeOffset.Parse("2021-12-13T00:00:00+00:00"),
                Page = 1,
                Rows = 20
            };

            var brandMerchant = new AviaBrandMerchant()
            {
                BrandCode = "Default",
                ApiUrl = new("https://api.aviaapi.vip"),
                Authorization = "81e2d1a24c684a76a710a70f4900a353",
                PlayerPassword = "TgsP@ssw0rd"
            };

            bool success = true;
            AviaGameRecordData[] datas = new AviaGameRecordData[1] {
                    new AviaGameRecordData {
                        Type= "Combo",
                        ComboType= "Combo",
                        OrderID= "136122074377333076",
                        UserName= "4ucmqplxb029",
                        Status= "Win",
                        CreateAt= "2021/12/9 13:45:43",
                        UpdateAt= "2021/12/10 1:13:26",
                        Timestamp= 1639070006598,
                        RewardAt= "2021/12/10 1:13:26",
                        BetMoney= 10.00m,
                        Money=15.09m,
                        OddsType= "EU",
                        BetAmount= 10.00m,
                        Odds=2.5113m,
                        IP= "220.132.206.70",
                        Language="CHN",
                        Platform= new string[2] {"PC",  " MAC" },
                        ReSettlement= 0,
                        IsTest= 0,
                        Currency= "CNY",
                        Details=new AviaGameRecordDetail[2] {
                            new AviaGameRecordDetail{
                                DetailID="136122074377533955",
                                CateID="10",
                                Category="刀塔II",
                                LeagueID="4425",
                                League="i-联赛",
                                MatchID= "5733",
                                Match="Xtreme Gaming VS Intense Gaming",
                                BetID= "27513",
                                Bet="地图1 - 获胜者",
                                Content= "XG",
                                ResultAt= "2021/12/10 0:57:45",
                                Result= "XG",
                                OddsType= "EU",
                                Odds="1.610",
                                Status= "Win"},
                            new AviaGameRecordDetail() {
                                DetailID= "136122074377533965",
                                CateID= "1",
                                Category="绝地求生",
                                LeagueID= "2080",
                                League="虎牙天命杯",
                                MatchID= "5736",
                                Match= "PUBG Global Invitational  VS SP",
                                BetID= "27507",
                                Bet= "地图1 - 排名第一队伍",
                                Content= "",
                                ResultAt= "2021/12/10 0:56:28",
                                Result="",
                                OddsType="EU",
                                Odds= "1.559",
                                Status="Win" }
                        }
                    }};

            _ = fakeAviaService.GetBrandMerchantAsync(query.BrandCode).Returns(ValueTask.FromResult(brandMerchant));

            var gameProviderCode = GameProviderType.Avia;
            var gameProviderId = Guid.Parse("fb9cc4ef-624f-46fb-8f17-a17c273bed9e");

            _ = fakeAviaService.GetGameProviderCode().Returns(gameProviderCode);
            _ = fakeAviaService.GetGameProviderIdAsync().Returns(gameProviderId);

            _ = fakeAviaApiClient.GetGameRecordAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                query.StartTime.ToOffset(TimeSpan.FromHours(8)),
                query.EndTime.ToOffset(TimeSpan.FromHours(8)),
                query.Page,
                query.Rows).Returns(ValueTask.FromResult<(bool Success, AviaGameRecordData[]? GameRecords)>((success, datas)));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(success, actual.Success);
            Assert.AreEqual(gameProviderCode, actual.Datas[0].GameProviderCode);
            Assert.AreEqual(gameProviderId, actual.Datas[0].GameProviderId);
            Assert.AreEqual(datas[0].OrderID, actual.Datas[0].ExternalRoundId);
        }
    }
}
