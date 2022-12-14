using System;

namespace Tgs.GameProviders.Client;

public record SM01PlatformLoginResponse(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey);
