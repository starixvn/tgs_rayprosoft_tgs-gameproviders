using NSubstitute;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories.UnitTest;

[TestClass]
public class PlayerRepositoryUintTest
{
    [TestMethod]
    public async Task Get_Player()
    {
        var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

        var playerCode = "test";

        _ = fakeClient.PlayerGetAsync(playerCode).Returns(new PlayerModel(
             ResellerId: Guid.NewGuid(),
             ResellerCode: "TGS",
             LicenseeId: Guid.NewGuid(),
             LicenseeCode: "TGS",
             BrandId: Guid.NewGuid(),
             BrandCode: "GPI",
             PlayerCode: playerCode,
             Username: playerCode,
             DefaultWalletId: Guid.NewGuid(),
             DefaultWalletCurrencyCode: "CNY",
             Status: PlayerStatus.Enabled));

        var repository = new PlayerRepository(fakeClient);

        var actual = await repository.GetAsync(playerCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(playerCode, actual.PlayerCode);
    }
}
