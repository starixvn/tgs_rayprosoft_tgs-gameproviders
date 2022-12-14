using System;

namespace Tgs.GameProviders.Core.SM01.ApplicationServices;

public record SM01PlatformLoginInfo(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey);
