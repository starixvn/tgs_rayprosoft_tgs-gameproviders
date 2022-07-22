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
public class EvolutionTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
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
            CasinoID = "babylonpega20001",
            Ua2Token = "test123",
            GameHistoryApiToken = "test123",
            ExternalLobbyApiToken = "test123",
            ecID = "babylonpega20001test123"
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

        var query = new EvolutionTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var token = "token";

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeToken.EncryptAsync(query.LoginRecordId).Returns(token);

        var response = new GetBalanceReply()
        {
            Result = true,
            IsPlayerExist = true,
            PlayerCode = query.PlayerCode,
            Uid = "",
            TBalance = 100m,
            ABalance = 100m
        };
        _ = fakeGameInterfaceEvolutionServiceGrpc.GetBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.ecID),
            playerCode: Arg.Is(query.PlayerCode)).Returns(response);

        var sut = new EvolutionTransferPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeGameInterfaceEvolutionServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository,
            fakeToken);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.ABalance.ToDecimal(), actual.Value);
    }

    [Test]
    public async Task Can_Get_Balance_When_Player_Is_Not_Exists()
    {
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
            CasinoID = "babylonpega20001",
            Ua2Token = "test123",
            GameHistoryApiToken = "test123",
            ExternalLobbyApiToken = "test123",
            ecID = "babylonpega20001test123"
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

        var query = new EvolutionTransferPlayerBalanceQuery
        {
            GameId = 1,
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1"
        };

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

        var token = "token";

        var gameProviderCultureCode = "en-US";
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeToken.EncryptAsync(query.LoginRecordId).Returns(token);
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
            Arg.Is(gameModel.GameProviderId),
            Arg.Is(query.Language)).Returns(gameProviderCultureCode);

        var response = new GetBalanceReply()
        {
            Result = true,
            IsPlayerExist = true,
            PlayerCode = query.PlayerCode,
            Uid = "",
            TBalance = 100m,
            ABalance = 100m
        };

        var notExistsResponse = new GetBalanceReply()
        {
            Result = false,
            IsPlayerExist = false,
            PlayerCode = "",
            Uid = "",
            TBalance = 0m,
            ABalance = 0m
        };

        _ = fakeGameInterfaceEvolutionServiceGrpc.GetBalanceAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            agentId: Arg.Is(configuration.ecID),
            playerCode: Arg.Is(query.PlayerCode)).Returns(notExistsResponse);

        var gameUrlResponse = new GameUrlReply()
        {
            Entry = "/entry",
            EntryEmbedded = "/EntryEmbedded",
            Errors = { new RepeatedField<Error>() }
        };

        _ = fakeGameInterfaceEvolutionServiceGrpc.GetSlotGameUrlAsync(
            ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
            playerCode: playerModel.PlayerCode,
            platformType: (short)query.PlatformType,
            ipAddress: query.IPAddress,
            encryptedToken: token,
            externalGameId: gameModel.GameExternalId,
            externalCultureCode: query.CurrencyCode,
            externalCurrencyCode: query.CurrencyCode,
            cancellationToken: default
        ).Returns(gameUrlResponse);

        var sut = new EvolutionTransferPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeGameInterfaceEvolutionServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository,
            fakeToken);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.ABalance.ToDecimal(), actual.Value);

        await fakeGameInterfaceEvolutionServiceGrpc.Received(2).GetBalanceAsync(Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.ecID), Arg.Is(query.PlayerCode));
    }
}
