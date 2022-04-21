using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Joker.Models;

public class JokerGameRecordData
{
    [JsonPropertyName("OCode")]
    public string TransactionId { get; init; } = default!;

    [JsonPropertyName("Username")]
    public string Username { get; init; } = default!;

    [JsonPropertyName("GameCode")]
    public string GameExternalId { get; init; } = default!;

    [JsonPropertyName("Description")]
    public string Description { get; init; } = default!;

    [JsonPropertyName("Type")]
    public string Type { get; init; } = default!;

    [JsonPropertyName("Amount")]
    public decimal BetAmount { get; init; } = default!;

    [JsonPropertyName("Result")]
    public decimal SettleAmount { get; init; } = default!;

    [JsonPropertyName("Time")]
    public DateTimeOffset Time { get; init; } = default!;
}