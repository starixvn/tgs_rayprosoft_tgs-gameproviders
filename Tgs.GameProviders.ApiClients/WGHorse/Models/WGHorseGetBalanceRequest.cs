namespace Tgs.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorseGetBalanceRequest
    {
        public string PlayerCode { get; init; } = default!;
    }
}
