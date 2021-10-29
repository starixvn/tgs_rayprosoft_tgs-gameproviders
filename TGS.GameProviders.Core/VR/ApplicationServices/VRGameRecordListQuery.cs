using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public record VRGameRecordListQuery
    {
        public string BrandCode { get; init; }

        public DateTimeOffset StartTime { get; init; }

        public DateTimeOffset EndTime { get; init; }

        public bool IsUpdateTime { get; init; }

        public int Page { get; init; }

        public int Rows { get; init; }
    }
}
