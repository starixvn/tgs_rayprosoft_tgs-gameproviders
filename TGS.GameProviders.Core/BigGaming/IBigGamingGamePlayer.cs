using System.Threading.Tasks;

namespace TGS.GameProviders.Core.BigGaming
{
    public interface IBigGamingGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(string brandCode, string playerCode, string currencyCode);
    }
}
