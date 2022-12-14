namespace Tgs.GameProviders.Client;

public record SM01PlatformLoginResponse(
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
