namespace Tgs.GameProviders.ApiClients.WGHorse.Models;

public record WGHorseLoginRequest
{
    public string Token { get; init; } = default!;
    public string Lang { get; init; } = default!;
}
