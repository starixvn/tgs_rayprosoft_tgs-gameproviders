using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming.Models;

namespace Tgs.GameProviders.ApiClients.SexyGaming
{
    internal class SexyGamingRoundDetailAnalyzeService : ISexyGamingRoundDetailAnalyzeService
    {
        private IReadOnlyDictionary<string, string> _pokerMapper;

        public SexyGamingRoundDetailAnalyzeService()
        {
            InitoPoker();
        }

        public string GetAnalyze(string externalGameId, SexyGamingGameRecord source)
          => externalGameId switch
          {
              "MX-LIVE-001" => GetBaccaratGameHistory(source),    //Baccarat Classic
              "MX-LIVE-002" => GetBaccaratGameHistory(source),    //Baccarat
              "MX-LIVE-003" => GetBaccaratGameHistory(source),    //Baccarat Insurance
              "MX-LIVE-006" => GetDragontigerGameHistory(source), //DragonTiger
              "MX-LIVE-007" => GetSicboGameHistory(source),       //SicBo
              "MX-LIVE-009" => JsonSerializer.Serialize(source.Result),  //Roulette
              "MX-LIVE-010" => GetRedBlueDuelGameHistory(source), //Red Blue Duel
              "MX-LIVE-011" => JsonSerializer.Serialize(source.Result),  //Twenty
              "MX-LIVE-012" => JsonSerializer.Serialize(source.Result),  //AndarBahar
              "MX-LIVE-014" => GetThaiHiLoGameHistory(source),  //Thai Hi Lo
              _ => GetBaccaratGameHistory(source)     //百家樂相關
          };

        private string GetBaccaratGameHistory(SexyGamingGameRecord source) =>
            JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {
                        "player", string.Join(',', _pokerMapper.Where(x => x.Key == source.Result[0]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[1]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[2]).First().Value)
                    },
                    {
                        "banker", string.Join(',', _pokerMapper.Where(x => x.Key ==  source.Result[3]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[4]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[5]).First().Value)
                    },
                    {"outcome",source.Winner}
                });

        private string GetCard(string cardIndex) => cardIndex switch
        {
            "H" => "♥",
            "C" => "♣",
            "D" => "♦",
            _ => "♠"
        };

        private string GetDragontigerGameHistory(SexyGamingGameRecord source) =>
            JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"dragon", _pokerMapper.Where(x => x.Key == source.Result[0]).First().Value},
                    {"tiger", _pokerMapper.Where(x => x.Key == source.Result[3]).First().Value},
                    {"outcome",source.Winner}
                });

        private string GetNumber(int value) => value switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => value.ToString()
        };

        private string GetRedBlueDuelGameHistory(SexyGamingGameRecord source)
        {
            var split = source.Result[0].Split(',');
            return JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"red", split[0]},
                    {"blue", split[1]},
                    {"outcome",source.Winner}
                });
        }

        private string GetSicboGameHistory(SexyGamingGameRecord source)
        {
            var split = source.Result[0].Split(',');
            return JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"dice1", split[0]},
                    {"dice2", split[1]},
                    {"dice3", split[2]},
                    {"outcome",source.Winner}
                });
        }

        private string GetThaiHiLoGameHistory(SexyGamingGameRecord source)
        {
            var split = source.Result[0].Split(',');
            return JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"dice1", split[0]},
                    {"dice2", split[1]},
                    {"dice3", split[2]},
                    {"outcome",source.Winner}
                });
        }

        private void InitoPoker()
        {
            var tempPokerMapper = new Dictionary<string, string>();
            tempPokerMapper.Add(string.Empty, 0.ToString("d2"));
            //黑桃= S, 愛心 = H, 方塊 = D, 梅花 = C;
            foreach (var c in new[]
            {
                "S",
                "H",
                "D",
                "C"
            })
            {
                for (var number = 1; number <= 13; number++)
                {
                    tempPokerMapper.Add($"{c}{number.ToString("d2")}", $"{GetCard(c)}{GetNumber(number)}");
                }
            }
            _pokerMapper = tempPokerMapper;
        }
    }
}
