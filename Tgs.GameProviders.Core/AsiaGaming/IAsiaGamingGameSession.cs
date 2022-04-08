using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AsiaGaming
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
