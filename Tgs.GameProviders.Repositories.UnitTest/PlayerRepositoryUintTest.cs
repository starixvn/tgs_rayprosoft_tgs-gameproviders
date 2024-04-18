using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Repositories.UnitTest;

public class PlayerRepositoryUintTest
{
	[Fact]
	public async Task Get_Player()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var playerCode = "test";

		_ = fakeClient.PlayerGetAsync(playerCode).Returns(new PlayerInfo(
			 ResellerId: Guid.NewGuid(),
			 ResellerCode: "TGS",
			 LicenseeId: Guid.NewGuid(),
			 LicenseeCode: "TGS",
			 BrandId: Guid.NewGuid(),
			 BrandCode: "GPI",
			 PlayerCode: playerCode,
			 Username: playerCode,
			 DefaultWalletId: Guid.NewGuid(),
			 DefaultWalletCurrencyCode: "CNY"));

		var repository = new PlayerRepository(fakeClient);

		var actual = await repository.GetAsync(playerCode);

		Assert.NotNull(actual);
		Assert.Equal(playerCode, actual.PlayerCode);
	}
}
