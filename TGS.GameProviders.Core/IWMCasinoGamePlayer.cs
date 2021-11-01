using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Core
{
    public interface IWMCasinoGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(Uri apiUrl, WMCasinoAgent agent, string wmPlayerId, string username);
    }
}