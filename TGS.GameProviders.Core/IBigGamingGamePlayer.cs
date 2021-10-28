using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface IBigGamingGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(string brandCode, string playerCode, string currencyCode);
    }
}
