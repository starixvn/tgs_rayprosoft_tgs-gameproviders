using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Core.Joker;

public interface IJokerGamePlayer
{
    ValueTask<bool> CreatePlayerAsync(string playerCode, Uri apiUrl, JokerAgent agent);
}