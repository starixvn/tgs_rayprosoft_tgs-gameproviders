using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Core.Avia.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class AviaApiClientUnitTest
    {
        private const string Authorization = "81e2d1a24c684a76a710a70f4900a353";
        private readonly Uri _apiUrl = new("https://api.aviaapi.vip");

        [Test]
        public async Task CreatePlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var password = "TgsP@ssw0rd";
            var currency = "CNY";

            var client = Substitute.For<IAviaApiClient>();

            _ = client.RegisterAsync(
                _apiUrl,
                Authorization,
                playerCode,
                password,
                currency).Returns(ValueTask.FromResult(true));

            var actual = await client.RegisterAsync(
                 _apiUrl,
                Authorization,
                playerCode,
                password,
                currency).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb023";
            var isPlayer = true;
            decimal? balance = 100m;

            var client = Substitute.For<IAviaApiClient>();

            _ = client.BalanceAsync(
                 _apiUrl,
                 Authorization,
                 playerCode).Returns(ValueTask.FromResult((isPlayer, balance)));

            var actual = await client.BalanceAsync(
                _apiUrl,
                Authorization,
                playerCode).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(balance, actual.Balance);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            DateTimeOffset startTime = DateTimeOffset.UtcNow.AddDays(-7).ToOffset(TimeSpan.FromHours(8));
            DateTimeOffset endTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            int pageIndex = 1;
            int pageSize = 20;

            bool success = true;
            AviaGameRecordData[]? datas = new AviaGameRecordData[1] {
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

            var client = Substitute.For<IAviaApiClient>();

            _ = client.GetGameRecordAsync(
                _apiUrl,
                Authorization,
                startTime,
                endTime,
                pageIndex,
                pageSize).Returns(ValueTask.FromResult<(bool Success, AviaGameRecordData[]? GameRecords)>((success, datas)));

            var actual = await client.GetGameRecordAsync(
                 _apiUrl,
                Authorization,
                startTime,
                endTime,
                pageIndex,
                pageSize).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(datas, actual.GameRecords);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var lang = "ENG";
            var cateId = default(int?);
            var matchId = default(int?);
            var isPlayer = true;
            Uri gameUrl = new Uri("https://www.google.com/");

            var client = Substitute.For<IAviaApiClient>();

            _ = client.GetGameUrlAsync(
                _apiUrl,
                Authorization,
                playerCode,
                lang,
                cateId,
                matchId).Returns(ValueTask.FromResult((isPlayer, (Uri?)gameUrl)));

            var actual = await client.GetGameUrlAsync(
                _apiUrl,
                Authorization,
                playerCode,
                lang,
                cateId,
                matchId).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(isPlayer, actual.IsPlayer);
            Assert.AreEqual(gameUrl, actual.GameUrl);
        }

        [Test]
        public async Task TransferInAsync()
        {
            var playerCode = "4ucmqplxb029";
            var currency = "CNY";
            var orderId = $"{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)):yyyyMMddHHmmssfff}{playerCode}";
            var amount = 100;

            var client = Substitute.For<IAviaApiClient>();

            _ = client.TransferInAsync(
                _apiUrl,
                Authorization,
                playerCode,
                currency,
                amount).Returns(ValueTask.FromResult((true, orderId)));

            var actual = await client.TransferInAsync(
                _apiUrl,
                Authorization,
                playerCode,
                currency,
                amount).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(orderId, actual.orderId);
        }

        [Test]
        public async Task TransferOutAsync()
        {
            var playerCode = "4ucmqplxb029";
            var currency = "CNY";
            var orderId = $"{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)):yyyyMMddHHmmssfff}{playerCode}";
            var amount = 100;

            var client = Substitute.For<IAviaApiClient>();

            _ = client.TransferOutAsync(
                _apiUrl,
                Authorization,
                playerCode,
                currency,
                amount).Returns(ValueTask.FromResult((true, orderId)));

            var actual = await client.TransferOutAsync(
                _apiUrl,
                Authorization,
                playerCode,
                currency,
                amount).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(orderId, actual.orderId);
        }
    }
}
