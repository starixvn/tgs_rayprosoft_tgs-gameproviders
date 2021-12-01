namespace TGS.GameProviders.Core.WGHorse.Models
{
    public record WGHorseGetBalanceRequest
    {
        public string PlayerCode { get; init; } = default!;
    }
}
