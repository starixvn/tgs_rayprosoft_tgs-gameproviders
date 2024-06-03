using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameProviders.Core.TPhere.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.TPhere;

public class TPhereKickPlayerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceTPhereClient>();

		var command = new TPhereKickPlayerCommand()
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.TPhere),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var player = new PlayerInfo(
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
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(gameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new TPhereConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.funkytest.com/"),
			ApiKey = "6309572cf90658e17847a7b021723106",
			ApiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
		};
		_ = fakeGameProviderRepository.GetTPhereConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var sut = new TPhereKickPlayerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		await sut.ExecuteAsync(command);

		_ = fakeClient
			.Received(1)
			.TP2PlayerLogoutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
