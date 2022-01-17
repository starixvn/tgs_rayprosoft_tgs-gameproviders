using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingGameSession
    {
        public ValueTask<bool> CreateGameSessionAsync(
            AsiaGamingConfiguration configuration,
            string playerCode,
            Guid loginRecordId,
            decimal playerBalance);
    }
}