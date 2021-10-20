using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public interface IGameProvidersClient
    {
        ValueTask KickPlayer(KickPlayerRequest source);

        ValueTask<PlayerBalanceResponse> PlayerBalance(PlayerBalanceRequest source);
    }
}
