using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeRoundService = Substitute.For<IRoundService>();
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();

        var configuration = new PragmaticPlayTransferConfiguration
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var gameProviderLanguage = "en";
        var historyUrl = "https://tgs.prerelease-env.biz/gs2c/parentRoundHistoryDetails.do?playSessionId=2806662804&stylename=tgs_tgstfw&hash=2213f4013b94f67808ff131af8110f7d&language=en";

        var playerInfo = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var gameProviderInfo = new GameProviderInfo(
            Guid.Parse("e4ecfa98-b373-4123-87e0-cb9e883d0691"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            27,
            false,
            "PPTF",
            "PragmaticPlay",
            null);

        var gameInfo = new GameInfo(
            2700027,
            GameStatus.Active,
            "vs25bkofkngdm",
            null,
            string.Empty,
            null,
            gameProviderInfo.GameProviderId,
            GameType.Slot,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Book_of_Kingdoms",
            "Book of Kingdoms",
            null,
            "PPTF");

        var data = new PragmaticPlayTransferGameRecordData
        {
            PlayerCode = "4ucmqplxb029",
            ExternalGameId = "vs25bkofkngdm",
            ExternalRoundId = "2806662804",
            ParentExternalRoundId = null,
            StartDate = "2022-05-13 08:04:41",
            EndDate = "2022-05-13 08:04:41",
            Status = "C",
            PlayType = "R",
            CurrencyCode = "CNY",
            BetAmount = 2.50m,
            WinAmount = 0.00m,
            Jackpot = 0.00m
        };

        var createdOn = DateTimeOffset.Parse(data.StartDate, null, DateTimeStyles.AssumeUniversal);
        var closedOn = DateTimeOffset.Parse(data.EndDate, null, DateTimeStyles.AssumeUniversal);

        var roundInfo = new RoundInfo(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            data.ExternalRoundId,
            gameProviderInfo.GameProviderId,
            playerInfo.PlayerCode,
            gameInfo.GameId,
            playerInfo.DefaultWalletCurrencyCode,
            RoundStatus.Closed,
            Guid.NewGuid(),
            playerInfo.DefaultWalletId,
            DateTime.Now,
            DateTime.Now,
            DateTime.Now,
            playerInfo.BrandId,
            createdOn,
            closedOn,
            data.BetAmount,
            data.BetAmount,
            data.WinAmount,
            null,
            data.BetAmount,
            data.BetAmount - data.WinAmount,
            null,
            RoundClientStatus.Loss,
            null,
            null,
            null,
            JsonSerializer.Serialize(data));

        var sut = new PragmaticPlayTransferRoundDetailGetService(
            fakeRoundService, fakeService, fakeApiClient);

        var query = new PragmaticPlayTransferRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        _ = fakeRoundService.GetAsync(Arg.Is(query.RoundId)).Returns(roundInfo);
        _ = fakeService.GetGameProviderAsync(Arg.Is(roundInfo.GameProviderId)).Returns(gameProviderInfo);
        _ = fakeService.GetGameAsync(Arg.Is(roundInfo.GameId)).Returns(gameInfo);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(
                Arg.Is(gameProviderInfo.GameProviderId),
                Arg.Is(query.Language))
            .Returns(gameProviderLanguage);

        _ = fakeApiClient
            .GetGameDetailUrlAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is(roundInfo.PlayerCode),
                Arg.Is(roundInfo.ExternalRoundId),
                Arg.Is(gameInfo.GameExternalId),
                Arg.Is(gameProviderLanguage))
            .Returns(historyUrl);

        var expected = new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson
                {
                    Value = JsonSerializer.Serialize(new
                    {
                        Url = historyUrl,
                        Jackpot = data.Jackpot
                    })
                }
            }
        };

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual!.Data);
    }
}