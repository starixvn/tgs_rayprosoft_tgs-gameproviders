using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public record GameRecordListResponse
    {
        public bool Success { get; init; }
        public GameRecordListViewModle[]? Datas { get; init; }
    }
}
