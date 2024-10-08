using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class GameProviderRepository(
	IGameCommandClient client) : IGameProviderRepository
{
	public async ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId, CancellationToken cancellationToken = default)
		=> await client.GameProviderGetAsync(
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(gameProviderId.ToString(), "Game Provider does not exist.");

	public async ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var data = await client
			.GameProviderConfigurationGetAsync(
			gameProviderConfigId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), "Game Provider Configuration does not exist.");

		return (data.DesktopEndpoint, data.MobileEndpoint);
	}

	public async ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode, CancellationToken cancellationToken = default)
		=> await client
		.GameProviderGetCultureCodeAsync(
		gameProviderId,
		cultureCode,
		cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode, CancellationToken cancellationToken = default)
		=> await client
		.GameProviderGetCurrencyCodeAsync(
		gameProviderId: gameProviderId,
		currencyCode: currencyCode,
		cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(currencyCode, "This game product currency does not exist.");

	public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode, CancellationToken cancellationToken = default)
		=> await client
		.GameProviderGetGameProviderIdAsync(
		gameProviderCode: gameProviderCode,
		cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(gameProviderCode, "Game Provider does not exist.");

	public ValueTask<SabaSWConfiguration> GetSabaSWConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<SabaSWConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.SabaSW,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<SabaPlusConfiguration> GetSabaPlusConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<SabaPlusConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.SabaPlus,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	private async ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string gameProviderConfiguration, Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var settingConfiguration = await client
			.SettingGetConfigurationAsync<TConfiguration>(
			key: gameProviderConfiguration,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(gameProviderConfiguration, "This game product setting does not exist.");

		var gameProviderConfig = await client
			.GameProviderConfigurationGetAsync(
			gameProviderConfigId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist.");

		return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
			? settingConfiguration
			: JsonMergeUtils.MergeModel(
				original: settingConfiguration,
				patch: gameProviderConfig.ConfigurationData,
				options: new JsonSerializerOptions()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					PropertyNameCaseInsensitive = true,
					Converters =
					{
						new JsonStringEnumConverter()
					}
				}
				);
	}
}
