using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.SexyGaming.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.ApiClients.SexyGaming;

namespace TGS.GameProviders.ApiClients
{
    public interface ISexyGamingApiClient
    {
        Task<SexyGamingCreateMemberResponse> CreatePlayerAsync(SexyGamingConfiguration configuration, SexyGamingCreateMemberRequest request);

        Task<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingGameUrlRequest request);

        Task<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request);

        Task<SexyGamingLogoutResponse> LogoutAsync(SexyGamingConfiguration configuration, SexyGamingLogoutRequest request);
    }
}
