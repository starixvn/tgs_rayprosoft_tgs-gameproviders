using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record AviaConfiguration : PluggableConfigurationBase
    {

        public decimal AmountMaximum { get; init; }

        public string AmountFormat { get; init; } = default!;


        public Uri ApiUrl { get; init; } = default!;

        public string Authorization { get; init; } = default!;

        public string PlayerPassword { get; init; } = default!;
    }
}
