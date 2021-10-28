using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public interface IGameProvidersClient
    {
        ValueTask<GameUrlResponse?> GameURLAsync(string gameProviderCode, GameUrlRequest source);

        ValueTask KickPlayerAsync(string gameProviderCode, KickPlayerRequest source);

        ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(string gameProviderCode, PlayerBalanceRequest source);

        ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, PlayerFundCheckingRequest source);

        ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, VRPlayerFundCheckingRequest source);

        ValueTask<PlayerFundResponse?> PlayerFundInAsync(string gameProviderCode, PlayerFundRequest source);

        ValueTask<PlayerFundResponse?> PlayerFundOutAsync(string gameProviderCode, PlayerFundRequest source);
    }
}
