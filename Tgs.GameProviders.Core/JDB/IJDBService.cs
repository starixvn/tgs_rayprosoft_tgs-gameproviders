using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.JDB;

public interface IJDBService
{
    ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode);

    ValueTask<JDBConfiguration> GetSettingConfigurationAsync();
}
