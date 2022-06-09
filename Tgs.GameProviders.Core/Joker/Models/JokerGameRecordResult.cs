using System;

#nullable enable

namespace Tgs.GameProviders.Core.Joker.Models;

public class JokerGameRecordResult
{
    public GameRecordListModel[] Datas { get; init; } = Array.Empty<GameRecordListModel>();

    public string? NextId { get; init; }

    public bool Success { get; init; }
}
