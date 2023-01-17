using ExpectedObjects;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Repositories.UnitTest;

[TestClass]
public class GameRepositoryUnitTest
{
    [TestMethod]
    public async Task Game_List()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameType = GameType.Fishing;

        var expected = new GameInfo[] {new (
            GameId: 1,
            GameCode: "9",
            GameProviderId: Guid.NewGuid(),
            "BG",
            GameExternalId: "Fishing",
            Type: GameType.Fishing,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "KY Fishing",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "Fishing",
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY") };
        _ = fakeClient.GameQueryAsync(gameProviderId, gameType).Returns(expected.ToAsyncEnumerable());

        var repository = new GameRepository(fakeClient);

        var actual = await repository.GetGameListAsync(gameProviderId, gameType).ToArrayAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Game()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameId = 1;

        _ = fakeClient.GameGetAsync(gameId).Returns(new GameInfo(
            GameId: 1,
            GameCode: "9",
            GameProviderId: Guid.NewGuid(),
            "KY",
            GameExternalId: "Fishing",
            Type: GameType.Fishing,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "KY Fishing",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "Fishing",
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY"));

        var repository = new GameRepository(fakeClient);

        var actual = await repository.GetAsync(gameId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameId, actual.GameId);
    }
}
