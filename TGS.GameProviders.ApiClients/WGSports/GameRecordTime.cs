using System.ComponentModel;

namespace TGS.GameProviders.ApiClients.WGSports
{
    public enum GameRecordTime
    {
        /// <summary>
        /// 下注时间
        /// </summary>
        [Description("bet_date")]
        BetDate,

        /// <summary>
        /// 结算时间
        /// </summary>
        [Description("settle_date")]
        SettleDate
    }
}
