namespace TGS.GameProviders.Core
{
    public record PlayerFundResult
    {
        /// <summary>
        /// The order identifier
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// The balance
        /// </summary>
        public decimal? Balance { get; set; }
    }
}
