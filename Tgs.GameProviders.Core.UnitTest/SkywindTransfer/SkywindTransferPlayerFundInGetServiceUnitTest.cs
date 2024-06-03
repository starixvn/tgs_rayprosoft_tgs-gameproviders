using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SkywindTransfer;

public class SkywindTransferPlayerFundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeSettingRepository = Substitute.For<ISettingRepository>();
		var fakeClient = Substitute.For<IGameInterfaceSkywindClient>();

		var query = new SkywindTransferPlayerFundInQuery
		{
			PlayerCode = "test",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 1
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.SkywindTransfer),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "test",
			Username: "test",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "HKD");
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var configuration = new SkywindTransferConfiguration
		{
			ApiUrl = new("http://localhost/"),
			LiveLobbyUrl = new("http://localhost/"),
			MerchId = "merchId",
			MerchPwd = "merchPwd",
			SecretKey = "secretKey",
			Username = "username",
			Password = "password"
		};
		_ = fakeSettingRepository.GetSkywindTransferConfigurationAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(configuration));

		var gameProviderCurrencyCode = "CNY";
		_ = fakeGameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(playerInfo.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderCurrencyCode));

		var request = new FundRequest()
		{
			ApiUrl = configuration.ApiUrl.ToString(),
			SecretKey = configuration.SecretKey,
			Username = configuration.Username,
			Password = configuration.Password,
			ExtTrxId = query.OrderId,
			PlayerCode = playerInfo.PlayerCode,
			Currency = gameProviderCurrencyCode,
			Amount = query.Amount,
		};
		var response = new FundReply()
		{
			Status = true,
			TrxId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
			ExtTrxId = query.OrderId,
			OrderStatus = "approved",
			Amount = query.Amount,
			Balance = 0
		};
		_ = fakeClient.FundInAsync(
			Arg.Is(request)).Returns(response);

		var sut = new SkywindTransferPlayerFundInGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeSettingRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal(response.TrxId, actual.ExternalOrderId);
	}
}
