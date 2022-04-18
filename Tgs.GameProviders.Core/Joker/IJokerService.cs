using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.Joker;

public interface IJokerService
{
    ValueTask<GameInfo> GetGameAsync(int gameId);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode);

    ValueTask<JokerConfiguration> GetSettingConfigurationAsync();
}