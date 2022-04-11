using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models.Commons.GameRecord
{
    public record VRGameRecordPrizeDetail
    {
        /// <summary>
        /// 中獎注數.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; init; }
        /// <summary>
        /// 中獎數字(內容有可能是中文).
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; init; }
        /// <summary>
        /// 中獎名稱(內容有可能是中文).
        /// </summary>
        [JsonPropertyName("awardName")]
        public string AwardName { get; init; }
    }
}
