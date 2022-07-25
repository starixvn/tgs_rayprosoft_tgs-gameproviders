using System;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.EvolutionTransfer;

[TestFixture]
public class EvolutionTransferGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();
        var fakeToken = Substitute.For<ITokenProvider>();

        var configuration = new EvolutionTransferConfiguration
        {
            Ua2AuthenticationUrl = new("https://babylonpega2.uat1.evo-test.com/ua/v1/babylonpega20001/test123"),
            ApiUrl = new("https://babylonpega2.uat1.evo-test.com"),
            CasinoId = "babylonpega20001",
            Ua2Token = "test123",
            GameHistoryApiToken = "test123",
            ExternalLobbyApiToken = "test123",
            EcId = "babylonpega20001test123"
        };

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);

        var gameModel = new GameModel(
            1,
            GameStatus.Active,
            "test",
            null,
            null,
            null,
            Guid.NewGuid(),
            GameType.Slot,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "test",
            "test",
            null,
            GameProviderType.EvolutionTransfer);

        var query = new EvolutionTransferGameUrlQuery
        {
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1"
        };

        var token = "token";
        var gameProviderCultureCode = "en-US";
        var gameProviderCurrencyCode = "CNY";

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameModel.GameProviderId);
        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameModel.GameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameModel.GameProviderId),
            Arg.Is(query.Language)).Returns(gameProviderCultureCode);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameModel);
        _ = fakeToken.EncryptAsync(query.LoginRecordId).Returns(token);
        _ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
            Arg.Is(gameModel.GameProviderId),
            Arg.Is(playerModel.DefaultWalletCurrencyCode)).Returns(gameProviderCurrencyCode);

        var response = new GameUrlReply()
        {
            Entry = "/entry",
            EntryEmbedded = "/EntryEmbedded",
            Errors = {new RepeatedField<Error>()}
        };
        _ = fakeGameInterfaceEvolutionServiceGrpc.GetSlotGameUrlAsync(
             ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
             playerCode: playerModel.PlayerCode,
             platformType: (short)query.PlatformType,
             ipAddress: query.IPAddress,
             encryptedToken: token,
             externalGameId: gameModel.GameExternalId,
             externalCultureCode: gameProviderCultureCode,
             externalCurrencyCode: query.CurrencyCode,
             cancellationToken: default
        ).Returns(response);

        var sut = new EvolutionTransferGameUrlGetService(
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeGameInterfaceEvolutionServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository,
            fakeToken);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        var path = response.Entry.Substring(1, response.Entry.Length - 1);

        var url = new Uri($"{configuration.ApiUrl}{path}");

        Assert.NotNull(actual);
        Assert.AreEqual(url, actual.Url);
    }

    [Test]
    public async Task Can_Get_Game_Url_With_Live_Dealer_Lobby()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();
        var fakeToken = Substitute.For<ITokenProvider>();

        var configuration = new EvolutionTransferConfiguration
        {
            Ua2AuthenticationUrl = new("https://babylonpega2.uat1.evo-test.com/ua/v1/babylonpega20001/test123"),
            ApiUrl = new("https://babylonpega2.uat1.evo-test.com"),
            CasinoId = "babylonpega20001",
            Ua2Token = "test123",
            GameHistoryApiToken = "test123",
            ExternalLobbyApiToken = "test123",
            EcId = "babylonpega20001test123"
        };

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);

        var gameModel = new GameModel(
            2,
            GameStatus.Active,
            "test",
            null,
            null,
            null,
            Guid.NewGuid(),
            GameType.LiveDealerLobby,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "test",
            "test",
            null,
            GameProviderType.EvolutionTransfer);

        var query = new EvolutionTransferGameUrlQuery
        {
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1"
        };

        var token = "token";
        var gameProviderCultureCode = "en-US";
        var gameProviderCurrencyCode = "CNY";

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameModel.GameProviderId);
        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameModel.GameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
             Arg.Is(gameModel.GameProviderId),
            Arg.Is(query.Language)).Returns(gameProviderCultureCode);
        _ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameModel);
        _ = fakeToken.EncryptAsync(query.LoginRecordId).Returns(token);
        _ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
            Arg.Is(gameModel.GameProviderId),
            Arg.Is(playerModel.DefaultWalletCurrencyCode)).Returns(gameProviderCurrencyCode);

        var response = new GameUrlReply()
        {
            Entry = "/entry",
            EntryEmbedded = "/EntryEmbedded",
            Errors = { new RepeatedField<Error>() }
        };
        _ = fakeGameInterfaceEvolutionServiceGrpc.GetLiveDealerGameUrlAsync(
             ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
             playerCode: playerModel.PlayerCode,
             platformType: (short)query.PlatformType,
             ipAddress: query.IPAddress,
             encryptedToken: token,
             externalCultureCode: gameProviderCultureCode,
             externalCurrencyCode: query.CurrencyCode,
             cancellationToken: default
        ).Returns(response);

        var sut = new EvolutionTransferGameUrlGetService(
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeGameInterfaceEvolutionServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository,
            fakeToken);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        var path = response.Entry.Substring(1, response.Entry.Length - 1);

        var url = new Uri($"{configuration.ApiUrl}{path}");

        Assert.NotNull(actual);
        Assert.AreEqual(url, actual.Url);
    }
}
