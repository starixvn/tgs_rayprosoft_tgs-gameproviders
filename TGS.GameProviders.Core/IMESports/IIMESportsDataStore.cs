using System.Threading.Tasks;

namespace TGS.GameProviders.Core.IMESports
{
    public interface IIMESportsDataStore
    {
        ValueTask CacheSessionTokenAsync(
            string playerCode,
            string gameExternalId,
            string sessionToken,
            int timeToLeave);
    }
}
