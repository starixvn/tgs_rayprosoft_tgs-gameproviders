using System;

#nullable enable

namespace Tgs.GameProviders.Core.Joker.Models;

public class JokerGameRecordResult
{
    public bool Success { get; init; }
    public string? NextId { get; init; }
    public GameRecordListModel[] Datas { get; init; } = Array.Empty<GameRecordListModel>();
}