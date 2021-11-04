using System.Threading.Tasks;

namespace TGS.GameProviders.Core.Skywind
{
    public interface ISkywindAccessToken
    {
        ValueTask<string> GetAccessTokenAsync();
    }
}
