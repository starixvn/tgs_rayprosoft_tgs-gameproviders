using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.PragmaticPlay
{
    public interface IPragmaticPlayService
    {
        ValueTask<GameInfo> GetGameAsync(int gameId);

        ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

        ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode);

        ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync();
    }
}
