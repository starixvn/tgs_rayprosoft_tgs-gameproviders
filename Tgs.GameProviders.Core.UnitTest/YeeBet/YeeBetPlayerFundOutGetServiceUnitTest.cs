using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameInterface.YeeBet.Protos;
using Tgs.GameProviders.Core.YeeBet.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.YeeBet;

public class YeeBetPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceYeeBetClient>();

		var query = new YeeBetPlayerFundOutQuery
		{
			PlayerCode = "4ucmqplxb029",
			OrderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
			Amount = 100M
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.YeeBet),
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

		var configuration = new YeeBetConfiguration()
		{
			ApiUrl = new("http://api.98kbus.top/"),
			AppId = "xtdA15OTVGEP",
			SecretKey = "4885366DF30D880224FF6AA46FBDE4F0",
		};
		_ = fakeGameProviderRepository.GetYeeBetConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var response = new FundReply()
		{
			TransactionId = query.OrderId.ToString(),
			Balance = 168M,
			Res = "{}"
		};
		_ = fakeClient
			.FundOutAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				appId: Arg.Is(configuration.AppId),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(player.PlayerCode),
				orderId: Arg.Is(long.Parse(query.OrderId)),
				amount: Arg.Is(query.Amount),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new YeeBetPlayerFundOutGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal(response.TransactionId, actual.ExternalOrderId);
	}
}
