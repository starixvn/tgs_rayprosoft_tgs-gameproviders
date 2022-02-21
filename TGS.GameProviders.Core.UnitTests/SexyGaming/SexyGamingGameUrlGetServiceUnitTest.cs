using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core.SexyGaming;
using TGS.GameProviders.Core.SexyGaming.ApplicationServices;
using TGS.GameProviders.Core.SexyGaming.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.SexyGaming;

namespace TGS.GameProviders.Core.UnitTests.SexyGaming
{
    [TestFixture]
    public class SexyGamingGameUrlGetServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<SexyGamingGameUrlGetService>.Instance;
            var fakeAviaApiClient = Substitute.For<ISexyGamingApiClient>();
            var fakeAviaService = Substitute.For<ISexyGamingService>();
            var fakeGamePlayerService = Substitute.For<ISexyGamingGamePlayer>();
            var fakeTokenProvider = Substitute.For<ITokenProvider>();

            var fakeService = new SexyGamingGameUrlGetService(fakeAviaApiClient, faceLogger, fakeAviaService, fakeTokenProvider, fakeGamePlayerService);

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
            _ = fakeAviaService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
            _ = fakeAviaService.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).Returns(ValueTask.FromResult(gameProviderCurrencyCode));
            _ = fakeAviaService.GetGameProviderCultureCodeAsync(query.Language).Returns(ValueTask.FromResult(gameProviderCultureCode));
            _ = fakeAviaApiClient.GetGamingGameUrlAsync(
                configuration.ApiUrl,
              new SexyGamingGameUrlRequest()
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
            Assert.AreEqual(gameUrl, actual.Url);
        }
    }
}
