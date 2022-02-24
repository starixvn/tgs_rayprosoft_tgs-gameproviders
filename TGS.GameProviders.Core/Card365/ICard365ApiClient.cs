using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Card365.Model;

namespace TGS.GameProviders.Core.Card365;

public interface ICard365ApiClient
{
    ValueTask<Card365GetKeyResponse> GetAccessKeyAsync(Uri apiUrl, string website, Card365GetKeyRequest request);

    ValueTask<Card365LogoutResponse> LogoutAsync(Uri apiUrl, string website, Card365LogoutRequest request);
}
