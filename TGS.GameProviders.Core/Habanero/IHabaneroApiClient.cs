using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Habanero.Models;

namespace TGS.GameProviders.Core.Habanero;

public interface IHabaneroApiClient
{
    ValueTask<HabaneroLogoutResponse> LogoutAsync(
        Uri apiUrl,
        HabaneroLogoutRequest request);
}
