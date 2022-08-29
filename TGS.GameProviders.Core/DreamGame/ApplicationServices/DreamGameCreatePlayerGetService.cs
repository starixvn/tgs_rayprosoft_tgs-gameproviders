using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

internal class DreamGameCreatePlayerGetService : IGetService<DreamGameCreatePlayerQuery, int>
{
    private readonly IDreamGameApiClient _client;

    public DreamGameCreatePlayerGetService(IDreamGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<int> GetAsync(DreamGameCreatePlayerQuery query, CancellationToken cancellationToken = default)
    {
        var tableLimit = query.BetLimits.Where(x => string.Equals(x.Currency, query.CurrencyCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().TableLimit;
        if (string.IsNullOrEmpty(tableLimit))
        {
            throw new Exception($"Currency limit red does not exist.({query.CurrencyCode})");
        }
        return _client.SignUpAsync(
                query.ApiUrl,
                query.ApiKey,
                query.AgentName,
                query.PlayerCode,
                query.Password,
                query.GameProviderCurrencyCode,
                tableLimit: tableLimit);
    }
}
