using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace Tgs.GameProviders.Core.WMCasino;

public interface IWMCasinoGamePlayer
{
    ValueTask<bool> CreatePlayerAsync(Uri apiUrl, WMCasinoAgent agent, string wmPlayerId, string username);
}
