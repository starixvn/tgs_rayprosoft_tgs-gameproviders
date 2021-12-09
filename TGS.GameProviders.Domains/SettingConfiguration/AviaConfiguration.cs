using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record AviaConfiguration : PluggableConfigurationBase
    {

        public decimal AmountMaximum { get; set; }

        public string AmountFormat { get; set; }


        public Uri ApiUrl { get; init; }

        public string Authorization { get; set; }
    }
}
