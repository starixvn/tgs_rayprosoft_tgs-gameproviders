using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.Joker;

public interface IJokerService
{
    ValueTask<GameModel> GetGameAsync(int gameId);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode);

    ValueTask<JokerConfiguration> GetSettingConfigurationAsync();
}
