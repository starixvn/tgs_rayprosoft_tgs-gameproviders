using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.IMESports;

public interface IIMESportsService
{
    ValueTask<GameModel> GetGameAsync(int gameId);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PlayerModel> GetPlayerAsync(string playerCode);

    ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync();
}
