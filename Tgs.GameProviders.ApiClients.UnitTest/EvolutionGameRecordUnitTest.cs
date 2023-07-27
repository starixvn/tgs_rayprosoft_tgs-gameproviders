using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.Evolution;
using Tgs.GameProviders.Core.Evolution.Models;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGameUrl;
using Tgs.GameProviders.Core.Extensions;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class EvolutionGameRecordUnitTest
{
    private readonly Uri _apiUrl = new("https://babylonqm.uat1.evo-test.com/");
    private readonly string _casinoID = "babylonqm0000001";
    private readonly string _gameHistoryApiToken = "test123";
    private readonly Uri _ua2AuthenticationUrl = new("https://babylonqm.uat1.evo-test.com/ua/v1/babylonqm0000001/test123");

    [Fact]
    public async Task GetGameUrlAsync()
    {
        var request = new EvolutionGameUrlRequest()
        {
            Uuid = GuidExtensions.NewSequentialGuid().ToString(),
            Player = new EvolutionGameUrlPlayer()
            {
                Id = "4ucmqplxb029",
                Update = false,
                FirstName = string.Empty,
                LastName = string.Empty,
                Nickname = "4ucmqplxb029",
                Country = "CN",
                Language = "cn",
                Currency = "CNY",
                Session = new EvolutionGameUrlPlayerSession()
                {
                    Id = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc",
                    Ip = "127.0.0.1"
                },
                Group = default
            },
            Config = new EvolutionGameUrlConfig()
            {
                Game = new EvolutionGameUrlConfigGame()
                {
                    Category = "baccarat_sicbo",
                    Table = null
                },
                Channel = new EvolutionGameUrlConfigChannel()
                {
                    Wrapped = true,
                    Mobile = false
                }
            }
        };

        IEvolutionApiClient client = Substitute.For<IEvolutionApiClient>();

        _ = await client.GetGameUrlAsync(_ua2AuthenticationUrl, request);
    }

    [Fact]
    public async Task GetGamHistoryRoundAsync()
    {
        var request = new EvolutionGamHistoryRoundRequest()
        {
            PlayerCode = "4uerjhy1ls17",
            RoundId = "3563277-56"
        };

        IEvolutionApiClient client = Substitute.For<IEvolutionApiClient>();

        _ = await client.GetGamHistoryRoundAsync(_apiUrl, _casinoID, _gameHistoryApiToken, request);
    }
}
