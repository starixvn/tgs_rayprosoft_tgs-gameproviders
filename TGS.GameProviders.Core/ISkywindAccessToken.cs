using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface ISkywindAccessToken
    {
        ValueTask<string> GetAccessTokenAsync();
    }
}
