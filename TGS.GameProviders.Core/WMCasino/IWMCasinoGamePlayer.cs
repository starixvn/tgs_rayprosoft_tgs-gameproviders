using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Core.WMCasino
{
    public interface IWMCasinoGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(Uri apiUrl, WMCasinoAgent agent, string wmPlayerId, string username);
    }
}
