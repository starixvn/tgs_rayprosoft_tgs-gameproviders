using System;
using System.Text.Json.Nodes;

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameInterface.TPhere.Protos;
using Tgs.GameProviders.Core.TPhere.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.TPhere;

public class TPherePlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeISnowflakeWork = Substitute.For<ISnowflakeWork>();
		var fakeClient = Substitute.For<IGameInterfaceTPhereClient>();

		var query = new TPherePlayerFundOutQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 100
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
			playerCode: Arg.Is(query.PlayerCode),
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
			ApiUrl = new("http://trial-gp-api.TPheretest.com/"),
			ApiKey = "6309572cf90658e17847a7b021723106",
			ApiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
		};
		_ = fakeGameProviderRepository.GetTPhereConfiguration(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		_ = fakeISnowflakeWork.GetId()
			.Returns(id);

		var reply = new FundReply
		{
			TransactionId = $"D{id}",
			CasinoTransId = Guid.NewGuid().ToString("N"),
			Balance = 0.0m,
			Res = new JsonObject { }.ToJsonString()
		};
		_ = fakeClient.TP2FundOutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			orderId: Arg.Is($"D{id}"),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new TPherePlayerFundOutGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeISnowflakeWork,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.Equal($"D{id}", actual.OrderId.ToString());
	}
}
