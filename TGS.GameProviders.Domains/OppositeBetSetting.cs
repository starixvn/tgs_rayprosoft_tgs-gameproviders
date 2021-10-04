using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Domains
{
    public record OppositeBetSetting
    {
        // game external ids
        public string[] Games { get; init; }

        public Pattern[] Patterns { get; init; }

        public record Pattern(string BetType, int GroupId);
    }
}
