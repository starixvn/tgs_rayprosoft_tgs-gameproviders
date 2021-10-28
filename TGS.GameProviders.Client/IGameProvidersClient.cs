using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public interface IGameProvidersClient
    {
        ValueTask KickPlayer(string gameProviderCode, KickPlayerRequest source);

        ValueTask<PlayerBalanceResponse> PlayerBalance(string gameProviderCode, PlayerBalanceRequest source);
    }
}
