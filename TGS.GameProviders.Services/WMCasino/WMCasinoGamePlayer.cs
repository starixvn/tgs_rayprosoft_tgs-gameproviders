using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Core.WMCasino.Models;
using TGS.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Services.WMCasino
{
    public class WMCasinoGamePlayer : IWMCasinoGamePlayer
    {
        private readonly IWMCasinoApiClient _client;
        private readonly ILogger<WMCasinoGamePlayer> _logger;

        public WMCasinoGamePlayer(
            ILogger<WMCasinoGamePlayer> logger,
            IWMCasinoApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CreatePlayerAsync(
            Uri apiUrl,
            WMCasinoAgent agent,
            string wmPlayerId,
            string username)
        {
            var request = new WMCasinoMemberRegisterRequest
            {
                VendorId = agent.AgentId,
                Signature = agent.Signature,
                User = wmPlayerId,
                Username = username.Length > 30 ? username[..30] : username
            };

            var response = await _client.MemberRegisterAsync(apiUrl, request).ConfigureAwait(false);

            return response.ErrorCode == (int)WMCasinoApiErrorCode.Success;
        }
    }
}
