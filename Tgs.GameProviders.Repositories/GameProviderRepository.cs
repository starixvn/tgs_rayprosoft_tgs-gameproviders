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

	public ValueTask<AVIAGamingConfiguration> GetAVIAGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AVIAGamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AVIAGaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcAelottoConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_AELOTTO,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcAwsConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_AWS,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcBgConfiguration> GetAwcBgConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcBgConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_BG,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcCockfightConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_COCKFIGHT,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcFacaiConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_FACAI,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcFsConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_FS,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcKmConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_KM,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcLudoConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_LUDO,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcPgsConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_PGS,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcRcbConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_RCB,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcSgConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_SG,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcSpadeConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_SPADE,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcVenusConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_VENUS,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcYesbingoConfiguration> GetAwcYesbingoConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcYesbingoConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_YESBINGO,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<AwcYlConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.AWC_YL,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<BTiSportsConfiguration> GetBTiSportsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<BTiSportsConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.BTiSports,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<Cards365TransferConfiguration> GetCards365TransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<Cards365TransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.Cards365Transfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<CQ9GamingConfiguration> GetCQ9GamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<CQ9GamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.CQ9Gaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<CrPatiConfiguration> GetCrPatiConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<CrPatiConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.CrPati,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<DreamGamingTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.DreamGamingTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<EndorphinaConfiguration> GetEndorphinaConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<EndorphinaConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.Endorphina,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<FaChaiConfiguration> GetFaChaiConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<FaChaiConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.FaChai,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<FunkyConfiguration> GetFunkyConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<FunkyConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.Funky,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<GA28Configuration> GetGA28ConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<GA28Configuration>(
			gameProviderConfiguration: GameProviderConfigurations.GA28,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

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

	public ValueTask<GKXGamingConfiguration> GetGKXGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<GKXGamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.GKXGaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<HabaneroTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.HabaneroTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<InplayMatrixESportsConfiguration> GetInplayMatrixESportsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<InplayMatrixESportsConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.InplayMatrix_ESports,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<JDBTransferConfiguration> GetJDBTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<JDBTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.JDBTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<JiliTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.JiliTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<JokerConfiguration> GetJokerConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<JokerConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.Joker,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<KaiyuanConfiguration> GetKaiyuanConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<KaiyuanConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.Kaiyuan,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<LEGamingConfiguration> GetLEGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<LEGamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.LEGaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<OBetConfiguration> GetOBetConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<OBetConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.OBet,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<PinnacleConfiguration> GetPinnacleConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<PinnacleConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.Pinnacle,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<PragmaticPlayTransferConfiguration> GetPragmaticPlayTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<PragmaticPlayTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.PragmaticPlayTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<SabaTransferConfiguration> GetSabaTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<SabaTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.SabaTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<SBOTransferConfiguration> GetSBOTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<SBOTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.SBOTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<SharpSportsConfiguration> GetSharpSportsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<SharpSportsConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.SharpSports,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<SM01Configuration> GetSM01ConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<SM01Configuration>(
			gameProviderConfiguration: GameProviderConfigurations.SM01,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<TFGamingTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.TFGamingTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<TPhereConfiguration> GetTPhereConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<TPhereConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.TPhere,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<V8PokerConfiguration> GetV8PokerConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<V8PokerConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.V8Poker,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<VRGamingConfiguration> GetVRGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<VRGamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.VRGaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<WGFmGamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.WGFmGaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<WGViaGamingConfiguration> GetWGViaGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<WGViaGamingConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.WGViaGaming,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<WicketCricketConfiguration> GetWicketCricketConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<WicketCricketConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.WicketCricket,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<WMCasinoTransferConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.WMCasinoTransfer,
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public ValueTask<YeeBetConfiguration> GetYeeBetConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> GetConfigurationAsync<YeeBetConfiguration>(
			gameProviderConfiguration: GameProviderConfigurations.YeeBet,
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
