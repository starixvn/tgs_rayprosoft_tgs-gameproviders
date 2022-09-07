using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.BigGaming.Models;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming;

public interface IBigGamingApiClient
{
    ValueTask<BigGamingResponse> CreateAgentAsync(Uri apiurl, string secretCode, string secretKey, BigGamingCreateAgentRequest request);

    ValueTask<BigGamingResponse> CreatePlayerAsync(Uri apiurl, string secretCode, string agentLoginIdPassword, BigGamingCreatePlayerRequest request);

    ValueTask<BigGamingResponse> GetGameRecordAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGameRecordFishingRequest request);

    ValueTask<BigGamingResponse> GetGameRecordFishingUrlAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGameRecordFishingUrlRequest request);

    ValueTask<BigGamingResponse> GetGameUrlAsync(Uri apiurl, string secretCode, string agentLoginIdPassword, BigGamingGetGameUrlLiveDealerRequest request);

    ValueTask<BigGamingResponse> GetGameUrlAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGetGameUrlFishingRequest request);

    ValueTask<BigGamingResponse> GetTrialGameUrlAsync(Uri apiurl, string secretCode, string secretKey, BigGamingGetTrialGameUrlRequest request);

    ValueTask<BigGamingResponse> GetVideoOrderDetailAsync(Uri apiurl, string secretKey, BigGamingVideoOrderDetailRequest request);

    ValueTask<BigGamingResponse> GetVideoOrderQueryAsync(Uri apiurl, string secretKey, BigGamingVideoOrderQueryRequest request);

    ValueTask<BigGamingResponse> GetVideoRoundQueryAsync(Uri apiurl, string secretKey, BigGamingVideoRoundQueryRequest request);

    ValueTask<BigGamingResponse> InfoPlayerAsync(Uri apiurl, string agentLoginIdPassword, BigGamingInfoPlayerRequest request);

    ValueTask<BigGamingResponse> KickPlayerAsync(Uri apiurl, string agentLoginIdPassword, BigGamingKickPlayerRequest request);

    ValueTask<BigGamingResponse> KickPlayerGameAsync(Uri apiurl, string agentLoginIdPassword, BigGamingKickPlayerGameRequest request);
}
