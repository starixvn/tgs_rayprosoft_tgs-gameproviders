using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.PragmaticPlay.Protos;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

public class PragmaticPlayTransferPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

		var query = new PragmaticPlayTransferPlayerFundOutQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 100
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.PragmaticPlayTransfer),
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
		_ = fakePlayerRepository
			.GetAsync(
				playerCode: Arg.Is(query.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository
			.GetBrandGameProviderConfigurationIdAsync(
				 gameProviderId: Arg.Is(gameProviderId),
				 brandId: Arg.Is(player.BrandId),
				 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new PragmaticPlayTransferConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			SecureLogin = "secure_login",
			SecretKey = "secret_key"
		};
		_ = fakeGameProviderRepository
			.GetPragmaticPlayTransferConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var transactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var fundReply = new FundReply
		{
			TransactionId = transactionId,
			Balance = 100,
			Res = JsonSerializer.Serialize(new
			{
				error = "0",
				description = "OK",
				transactionId,
				balance = 100,
			})
		};
		_ = fakeClient
			.FundOutAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				secureLogin: Arg.Is(configuration.SecureLogin),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(query.PlayerCode),
				orderId: Arg.Is(query.OrderId),
				amount: Arg.Is(query.Amount),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(fundReply);

		var sut = new PragmaticPlayTransferPlayerFundOutGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.GameProviderTransactionId);
		Assert.Equal(fundReply.TransactionId.ToString(), actual.ExternalGameProviderTransactionId);
	}
}
