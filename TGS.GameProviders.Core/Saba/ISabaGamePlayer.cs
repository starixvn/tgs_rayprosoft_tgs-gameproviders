using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Saba
{
    public interface ISabaGamePlayer
    {
        public ValueTask<bool> CreateMemberAsync(
            Guid gameProviderId,
            SabaConfiguration configuration,
            string sabaPlayerId,
            string currencyCode,
            bool isTestEnvironment);
    }
}