using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Habanero.Models;

namespace Tgs.GameProviders.Core.Habanero;

public interface IHabaneroApiClient
{
    ValueTask<HabaneroLogoutResponse> LogoutAsync(
        Uri apiUrl,
        HabaneroLogoutRequest request);
}
