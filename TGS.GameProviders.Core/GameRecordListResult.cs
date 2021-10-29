using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public record GameRecordListResult
    {
        public bool Success { get; init; }
        public GameRecordListModle[] Datas { get; init; }
    }
}
