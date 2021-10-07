using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.TCG
{
    public record TCGGameProviderScheduleConfig
    {
        public string LastGameRecorDirectory { get; set; }
        public string LastGameRecorFile { get; set; }
    }
}
