using System;

namespace TGS.GameProviders.Domains.SettingConfiguration.TCG
{
    public record TCGMerchant
    {
        /// <summary>
        /// The game URL
        /// </summary>
        public Uri GameUrl { get; init; } = default!;
        /// <summary>
        /// The currency code
        /// </summary>
        public string CurrencyCode { get; init; } = default!;
        /// <summary>
        /// The merchant code
        /// </summary>
        public string MerchantCode { get; init; } = default!;
        /// <summary>
        /// The DES key
        /// </summary>
        public string DESKey { get; init; } = default!;
        /// <summary>
        /// The sh a256 key
        /// </summary>
        public string SHA256Key { get; init; } = default!;
        /// <summary>
        /// The player password
        /// </summary>
        public string PlayerPassword { get; init; } = default!;
    }
}
