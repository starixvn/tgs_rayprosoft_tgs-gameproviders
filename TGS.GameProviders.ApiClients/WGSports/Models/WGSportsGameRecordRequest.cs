namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsGameRecordRequest
    {
        /// <summary>
        /// The game record time
        /// </summary>
        public GameRecordTime GameRecordTime { get; init; }

        /// <summary>
        /// The start time
        /// </summary>
        public string StartTime { get; init; }

        /// <summary>
        /// The end time
        /// </summary>
        public string EndTime { get; init; }
    }
}
