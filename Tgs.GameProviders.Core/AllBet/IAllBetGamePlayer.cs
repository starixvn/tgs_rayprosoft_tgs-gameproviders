using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.AllBet;

namespace Tgs.GameProviders.Core.AllBet;

public interface IAllBetGamePlayer
{
    ValueTask<bool> CheckOrCreateMemberAsync(string allBetPlayerId, Uri apiUrl, AllBetAgent agent);
}
