using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Domains.SettingConfiguration.DreamGame
{
    public record DreamGameBetLimit()
    {
        public string Currency { get; init; } = default!;

        public string TableLimit { get; init; } = default!;
    }
}
