namespace TGS.GameProviders.ViewModels
{
    public record PlayerFundResponse
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
