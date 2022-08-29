using ExpectedObjects;
using NSubstitute;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories.UnitTest;

[TestClass]
public class GameRepositoryUnitTest
{
    [TestMethod]
    public async Task Game_List()
    {
        var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

        var gameProviderId = Guid.NewGuid();
        var gameType = GameType.Fishing;

        var expected = new GameModel[] {new (
            GameId: 1,
            GameProviderId: Guid.NewGuid(),
            "BG",
            GameExternalId: "Fishing",
            Status: GameStatus.Archived,

            Description: "Fishing",
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,

            GameType: GameType.Fishing,
            PlatformType: GamePlatformType.DesktopAndMobile,
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            AvailableCurrencies: null,
            GameCode: "9",
            Name: "KY Fishing",
            Localization: null,
            SubGameProviderCode: "BG") };
        _ = fakeClient.GameQueryAsync(gameProviderId, gameType).Returns(expected);

        var repository = new GameRepository(fakeClient);

        var actual = await repository.GetGameListAsync(gameProviderId, gameType).ToArrayAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Game()
    {
        var fakeClient = Substitute.For<IGameProvidersQueryServerClient>();

        var gameId = 1;

        _ = fakeClient.GameGetAsync(gameId).Returns(new GameModel(
            GameId: gameId,
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "CardGameLobby",
            Status: GameStatus.Archived,
            Description: "CardGameLobby",
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            GameType: GameType.CardGameLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            AvailableCurrencies: null,
            GameCode: "8",
            Name: "KY CardGameLobby",
            Localization: null,
            SubGameProviderCode: "KY"));

        var repository = new GameRepository(fakeClient);

        var actual = await repository.GetAsync(gameId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameId, actual.GameId);
    }
}
