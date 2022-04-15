#nullable enable

using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core.Joker;

public interface IJokerApiClient
{
    ValueTask<bool> CreateUserAsync(Uri apiUrl, string playerCode, string appId, string secretKey);

    ValueTask<decimal?> GetBalanceAsync(Uri apiUrl, string playerCode, string appId, string secretKey);

    ValueTask<string?> GetGameTokenAsync(Uri apiUrl, string playerCode, string appId, string secretKey);
}