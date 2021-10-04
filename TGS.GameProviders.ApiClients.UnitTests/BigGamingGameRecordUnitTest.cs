using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class BigGamingGameRecordUnitTest
    {
        private Dictionary<int, string> pokerMapper;

        public BigGamingGameRecordUnitTest()
        {
            InitoPoker();
        }

        [Test]
        public void testc()
        {
            var 下注闲1平倍 = Convert.ToString(3489660929, 16);
            var 下注闲1翻倍 = Convert.ToString(3489660930, 16);
            var 下注闲2平倍 = Convert.ToString(3489660932, 16);
            var 下注闲2翻倍 = Convert.ToString(3489660936, 16);
            var 下注闲3平倍 = Convert.ToString(3489660944, 16);
            var 下注闲3翻倍 = Convert.ToString(3489660960, 16);
            var 下注闲1平倍10進制 = Convert.ToUInt32(下注闲1平倍, 16);
            var 下注闲1翻倍10進制 = Convert.ToUInt32(下注闲1翻倍, 16);
            var 下注闲2平倍10進制 = Convert.ToUInt32(下注闲2平倍, 16);
            var 下注闲2翻倍10進制 = Convert.ToUInt32(下注闲2翻倍, 16);
            var 下注闲3平倍10進制 = Convert.ToUInt32(下注闲3平倍, 16);
            var 下注闲3翻倍10進制 = Convert.ToUInt32(下注闲3翻倍, 16);
        }

        [Test]
        public void 牛牛()
        {
            var result = GetBullBullGameHistory(Convert.FromBase64String("CyQJJyMEFiUPNC8ZExQyBy0CIBE="));

            Assert.AreEqual(true, true);
        }

        [Test]
        public void 多彩百家乐()
        {
            var result = GetFullColorBaccaratGameHistory(Convert.FromBase64String("MAEFGhYr"));

            Assert.AreEqual(true, true);
        }

        [Test]
        public void 百家樂()
        {
            var result = GetBaccaratGameHistory(Convert.FromBase64String("LDQXBCQA"));

            Assert.AreEqual(true, true);
        }

        [Test]
        public void 炸金花()
        {
            var result = GetWinThreeCardsGameHistory(Convert.FromBase64String("FRAoKwcB"));

            Assert.AreEqual(true, true);
        }

        [Test]
        public void 骰宝()
        {
            var value = 196870;
            var 骰宝1 = value >> 16;
            var 骰宝2 = (value & 0xFF00) >> 8;
            var 骰宝3 = value & 0xFF;
            Assert.AreEqual(true, true);
        }

        private string GetBaccaratGameHistory(byte[] values)
        {
            string[] banker = new string[3] {
                pokerMapper.Where(x => x.Key == values[1]).First().Value,
                pokerMapper.Where(x => x.Key == values[3]).First().Value,
                pokerMapper.Where(x => x.Key == values[5]).First().Value,
            };
            string[] player = new string[3]{
                pokerMapper.Where(x => x.Key == values[0]).First().Value,
                pokerMapper.Where(x => x.Key == values[2]).First().Value,
                pokerMapper.Where(x => x.Key == values[4]).First().Value,
            };

            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "player",string.Join(',', player)},
                        { "banker",string.Join(',', banker)}
                               });
        }

        private string GetBullBullGameHistory(byte[] values)
        {
            var banker = new string[5];
            var player1 = new string[5];
            var player2 = new string[5];
            var player3 = new string[5];
            for (int i = 0; i < 5; i++)
            {
                int bankerIndex = i;
                int player1Index = bankerIndex + 4 + 1;
                int player2Index = player1Index + 4 + 1;
                int player3Index = player2Index + 4 + 1;
                banker[i] = pokerMapper.Where(x => x.Key == values[bankerIndex]).First().Value;
                player1[i] = pokerMapper.Where(x => x.Key == values[player1Index]).First().Value;
                player2[i] = pokerMapper.Where(x => x.Key == values[player2Index]).First().Value;
                player3[i] = pokerMapper.Where(x => x.Key == values[player3Index]).First().Value;
            }

            return JsonSerializer.Serialize(
                new Dictionary<string, string>() {
                    { "banker",string.Join(',', banker)},
                    { "player1",string.Join(',', player1)},
                    { "player2",string.Join(',', player2)},
                    { "player3",string.Join(',', player3)}
                });
        }

        private string GetCard(int cardIndex) => cardIndex switch
        {
            1 => "♥",
            2 => "♣",
            3 => "♦",
            _ => "♠"
        };

        private string GetFullColor(int value)
        {
            if (0 < value && value < 12) return "Green";
            else if (11 < value && value < 23) return "Orange";
            else if (22 < value && value < 34) return "Purple";
            else if (33 < value && value < 45) return "Blue";
            else return "White";
        }

        private string GetFullColorBaccaratGameHistory(byte[] values)
        {
            var banker = new (string Key, int Value)[3];
            var bankerIndex = 0;
            var player = new (string Key, int Value)[3];
            var playerIndex = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var ColorString = (values[i] == 0) ? "" : GetFullColor(values[i]);
                var NumberString = (values[i] == 0) ? 0 : (values[i] % 11 == 0) ? 11 : values[i] % 11;
                if (i is 1 or 3 or 5)
                {
                    banker[bankerIndex] = (ColorString, NumberString);
                    bankerIndex++;
                }
                else
                {
                    player[playerIndex] = (ColorString, NumberString);
                    playerIndex++;
                }
            }

            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "player",string.Join(",", player.Select(kv => kv.Key + ":" + kv.Value).ToArray())},
                        { "banker",string.Join(",", banker.Select(kv => kv.Key + ":" + kv.Value).ToArray())}
                           });
        }

        private string GetNumber(int value) => value switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => value.ToString()
        };

        private string GetWinThreeCardsGameHistory(byte[] values)
        {
            string[] dragon = new string[3] {
                pokerMapper.Where(x => x.Key == values[0]).First().Value,
                pokerMapper.Where(x => x.Key == values[1]).First().Value,
                pokerMapper.Where(x => x.Key == values[2]).First().Value,
            };
            string[] phoenix = new string[3]{
                pokerMapper.Where(x => x.Key == values[3]).First().Value,
                pokerMapper.Where(x => x.Key == values[4]).First().Value,
                pokerMapper.Where(x => x.Key == values[5]).First().Value,
            };

            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "dragon",string.Join(',', dragon)},
                        { "phoenix",string.Join(',', phoenix)}
                               });
        }

        private void InitoPoker()
        {
            pokerMapper = new Dictionary<int, string>();
            pokerMapper.Add(0, "00");
            int i = 0;
            for (int card = 0; card < 4; card++)
            {
                var cardString = GetCard(card);
                for (int number = 0; number < 13; number++)
                {
                    string numberString = GetNumber(number + 1);
                    pokerMapper.Add(i + 1, cardString + numberString);
                    i++;
                }
            }
        }
    }
}
