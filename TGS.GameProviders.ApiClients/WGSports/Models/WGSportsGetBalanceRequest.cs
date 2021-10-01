namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsGetBalanceRequest
    {
        public string PlayerCode { get; init; }
    }
}
