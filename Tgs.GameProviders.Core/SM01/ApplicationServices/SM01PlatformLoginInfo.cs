namespace Tgs.GameProviders.Core.SM01.ApplicationServices;

public record SM01PlatformLoginInfo(
	string ApiUrl,
	string MerchantId,
	string CustomerName,
	long Timestamp,
	string Sign);
