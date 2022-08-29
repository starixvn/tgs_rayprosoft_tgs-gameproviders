using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.Saba;

public interface ISabaGamePlayer
{
    public ValueTask<bool> CreateMemberAsync(
        Guid gameProviderId,
        SabaConfiguration configuration,
        string sabaPlayerId,
        string currencyCode);
}
