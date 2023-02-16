namespace Tgs.GameProviders.ViewModels;

public record SM01PlatformLoginResponse(
    string ApiUrl,
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
