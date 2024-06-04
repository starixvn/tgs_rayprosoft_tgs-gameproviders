using System.Text.Json.Nodes;

namespace Tgs.GameProviders.ViewModels;

public record PlayerFundResponse(
	string OrderId,
	string ExternalOrderId);