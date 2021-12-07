using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration.AllBet;

namespace TGS.GameProviders.Core.AllBet.ApplicationServices;

public interface IAllBetGamePlayer
{
    ValueTask<bool> CheckOrCreateMemberAsync(string allBetPlayerId, Uri apiUrl, AllBetAgent agent);
}