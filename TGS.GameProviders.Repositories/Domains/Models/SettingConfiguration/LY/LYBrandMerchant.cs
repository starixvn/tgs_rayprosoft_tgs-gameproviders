using System;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.LY
{
    public record LYBrandMerchant
    {
        /// <summary>
        /// The brand code
        /// </summary>
        public string BrandCode { get; init; }

        /// <summary>
        /// The API URL
        /// </summary>
        public Uri ApiUrl { get; init; }

        /// <summary>
        /// The API record URL
        /// </summary>
        public Uri ApiRecordUrl { get; set; }

        /// <summary>
        /// The deskey
        /// </summary>
        public string Deskey { get; set; }

        /// <summary>
        /// The md5key
        /// </summary>
        public string Md5key { get; set; }

        /// <summary>
        /// The agent mappers
        /// </summary>
        public LYAgentMapper[] AgentMappers { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        public bool Status { get; set; }
    }
}
