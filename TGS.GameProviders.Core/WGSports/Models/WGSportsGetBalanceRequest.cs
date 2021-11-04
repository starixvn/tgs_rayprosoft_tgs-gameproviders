namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsGetBalanceRequest
    {
        public string PlayerCode { get; init; }
    }
}
