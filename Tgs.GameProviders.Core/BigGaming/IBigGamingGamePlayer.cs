using System.Threading.Tasks;

namespace Tgs.GameProviders.Core.BigGaming
{
    public interface IBigGamingGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(string brandCode, string playerCode, string currencyCode);
    }
}
