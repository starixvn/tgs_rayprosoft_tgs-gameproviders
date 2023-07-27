using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming.ApplicationServices;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.SexyGaming;

namespace Tgs.GameProviders.Core.UnitTest.SexyGaming;

public class SexyGamingGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var faceLogger = NullLogger<SexyGamingGameUrlGetService>.Instance;
        var fakeApiClient = Substitute.For<ISexyGamingApiClient>();
        var fakeSexyGamingService = Substitute.For<ISexyGamingService>();
        var fakeGamePlayer = Substitute.For<ISexyGamingGamePlayer>();
        var fakeTokenProvider = Substitute.For<ITokenProvider>();

        var fakeService = new SexyGamingGameUrlGetService(fakeApiClient, faceLogger, fakeSexyGamingService, fakeTokenProvider, fakeGamePlayer);

        var query = new SexyGamingGameUrlQuery()
        {
            GameType = GameType.ESports,
            GameId = 500010,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "zh-CN",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var configuration = new SexyGamingConfiguration()
        {
            ServiceEndpointUrl = new("http://game-sg"),
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "tgs",
            Cert = "OFIHXL0mfK8AcohvyTs",
            BetLimit = new SexyGamingBetLimit()
            {
                SexybcrtMapper = new Dictionary<int, string>()
                {
                    { 280301,"CNY" },
                    { 280302,"CNY" },
                     { 280303,"CNY" },
                }
            }
        };

        var gameProviderCurrencyCode = "CNY";
        var gameProviderCultureCode = "CHN";
        Uri gameUrl = new("https://www.google.com/");

        var response = new SexyGamingGameUrlResponse()
        {
            Status = "0000",
            Desc = SexyGamingGameApiError.Success.ToString(),
            Url = gameUrl.ToString()
        };

        var encryptedToken = Guid.NewGuid().ToString("N");
        _ = fakeTokenProvider.EncryptAsync(query.LoginRecordId).Returns(ValueTask.FromResult(encryptedToken));
        _ = fakeSexyGamingService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeSexyGamingService.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).Returns(ValueTask.FromResult(gameProviderCurrencyCode));
        _ = fakeSexyGamingService.GetGameProviderCultureCodeAsync(query.Language).Returns(ValueTask.FromResult(gameProviderCultureCode));
        _ = fakeSexyGamingService.IsSuccess("0000").Returns(true);
        _ = fakeApiClient.GetGamingLoginGameAsync(
            configuration.ApiUrl,
          new SexyGamingLoginGameRequest()
          {
              Cert = configuration.Cert,
              AgentId = configuration.AgentId,
              UserId = query.PlayerCode,
              IsMobileLogin = query.PlatformType == PlatformType.Mobile,
              Language = gameProviderCultureCode,
              Extension1 = JsonSerializer.Serialize(new
              {
                  token = encryptedToken
              })
          }).Returns(ValueTask.FromResult(response));

        var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual.Url);
    }
}
