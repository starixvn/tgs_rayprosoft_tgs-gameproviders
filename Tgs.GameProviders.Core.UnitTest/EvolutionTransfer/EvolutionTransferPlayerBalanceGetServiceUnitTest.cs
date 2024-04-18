using System;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.EvolutionTransfer;

public class EvolutionTransferPlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Can_Get_Balance_When_Player_Is_Not_Exists()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeSettingRepository = Substitute.For<ISettingRepository>();

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

		var playerModel = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var query = new EvolutionTransferPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var token = "token";

		var gameProviderCurrencyCode = "CNY";
		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameProviderId);
		_ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			Arg.Is(gameProviderId),
			Arg.Is(playerModel.DefaultWalletCurrencyCode)).Returns(gameProviderCurrencyCode);

		var notExistsResponse = new GetBalanceReply()
		{
			Result = false,
			IsPlayerExist = false,
			PlayerCode = "",
			Uid = "",
			Balance = 0m
		};

		_ = fakeGameInterfaceEvolutionServiceGrpc.GetBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.EcId),
			playerCode: Arg.Is(query.PlayerCode)).Returns(notExistsResponse);

		var gameUrlResponse = new GameUrlReply()
		{
			Entry = "/entry",
			EntryEmbedded = "/EntryEmbedded",
			Errors = { new RepeatedField<Error>() }
		};

		_ = fakeGameInterfaceEvolutionServiceGrpc.GetLiveDealerGameUrlAsync(
			ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
			brandCode: playerModel.BrandCode,
			playerCode: playerModel.PlayerCode,
			platformType: (short)PlatformType.Desktop,
			ipAddress: "127.0.0.1",
			encryptedToken: token,
			externalCultureCode: "cn",
			externalCurrencyCode: gameProviderCurrencyCode,
			cancellationToken: default
		).Returns(gameUrlResponse);

		var sut = new EvolutionTransferPlayerBalanceGetService(
			fakeGameProviderRepository,
			fakeGameInterfaceEvolutionServiceGrpc,
			fakePlayerRepository,
			fakeSettingRepository);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.GetAsync(query).AsTask());

		_ = await fakeGameInterfaceEvolutionServiceGrpc
			.Received(2)
			.GetBalanceAsync(
			Arg.Is(configuration.ApiUrl),
			Arg.Is(configuration.EcId),
			Arg.Is(query.PlayerCode));
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeSettingRepository = Substitute.For<ISettingRepository>();

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

		var playerModel = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var query = new EvolutionTransferPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameProviderId);
		_ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

		var response = new GetBalanceReply()
		{
			Result = true,
			IsPlayerExist = true,
			PlayerCode = query.PlayerCode,
			Uid = "",
			Balance = 100m
		};
		_ = fakeGameInterfaceEvolutionServiceGrpc.GetBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.EcId),
			playerCode: Arg.Is(query.PlayerCode)).Returns(response);

		var sut = new EvolutionTransferPlayerBalanceGetService(
			fakeGameProviderRepository,
			fakeGameInterfaceEvolutionServiceGrpc,
			fakePlayerRepository,
			fakeSettingRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(response.Balance.ToDecimal(), actual.Balance);
	}
}
