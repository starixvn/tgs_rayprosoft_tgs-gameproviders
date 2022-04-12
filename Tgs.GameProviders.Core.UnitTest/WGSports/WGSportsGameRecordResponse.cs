using Tgs.GameProviders.Core.WGSports.Models;

namespace Tgs.GameProviders.Core.UnitTest.WGSports
{
    internal class WGSportsGameRecordResponse
    {
        public WGSportsBetInfo[] BetInfo { get; set; } = default!;

        public int ReturnCode { get; set; }

        public string Total { get; set; } = default!;

        public string TotalActiveAmount { get; set; } = default!;

        public string TotalAmount { get; set; } = default!;

        public string TotalResult { get; set; } = default!;
    }
}
