using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.JDB;

public interface IJDBService
{
    ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode);

    ValueTask<JDBConfiguration> GetSettingConfigurationAsync();
}