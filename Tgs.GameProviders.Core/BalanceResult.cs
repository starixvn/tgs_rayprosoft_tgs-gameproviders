using System;

namespace Tgs.GameProviders.Core;

public record BalanceResult(
    string PlayerCode,
    decimal Balance,
    DateTimeOffset LastDateTime);
