using TGS.GameProviders.ApiClients.VR.Models.Commons.GameRecord;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public record VRGameRecordListResult
    {
        public bool Success { get; init; }
        public VRGameRecord[] Datas { get; init; }
    }
}
