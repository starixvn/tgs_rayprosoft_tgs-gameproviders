namespace TGS.GameProviders.Client
{
    internal record PlayerFundResponse
    {
        /// <summary>
        /// The order identifier
        /// </summary>
        public string? OrderId { get; set; } = null;
    }
}
