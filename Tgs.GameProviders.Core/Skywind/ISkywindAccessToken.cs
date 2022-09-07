using System.Threading.Tasks;

namespace Tgs.GameProviders.Core.Skywind;

public interface ISkywindAccessToken
{
    ValueTask<string> GetAccessTokenAsync();
}
