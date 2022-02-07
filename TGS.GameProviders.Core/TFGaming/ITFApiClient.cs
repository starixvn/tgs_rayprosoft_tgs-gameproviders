using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.TFGaming
{
    public interface ITFApiClient
    {
        ValueTask<Uri> GetGameUrlAsync(Uri apiUrl, string publicToken, string playerCode);

        ValueTask KickAsync(Uri apiUrl, string privateToken, string partnerId, string playerCode);
    }
}
