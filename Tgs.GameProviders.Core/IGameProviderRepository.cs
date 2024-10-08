#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core;

public interface IGameProviderRepository
{
	ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId, CancellationToken cancellationToken = default);

	ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default);

	ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode, CancellationToken cancellationToken = default);

	ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode, CancellationToken cancellationToken = default);

	ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode, CancellationToken cancellationToken = default);

	ValueTask<SabaSWConfiguration> GetSabaSWConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default);

	ValueTask<SabaPlusConfiguration> GetSabaPlusConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default);
}
