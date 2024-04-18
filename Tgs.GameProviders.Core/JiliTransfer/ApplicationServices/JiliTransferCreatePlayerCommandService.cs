using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

internal class JiliTransferCreatePlayerCommandService : ICommandService<JiliTransferCreatePlayerCommand>
{
	private readonly IGameInterfaceJiliClient _client;

	public JiliTransferCreatePlayerCommandService(
		IGameInterfaceJiliClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public ValueTask ExecuteAsync(JiliTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
		=> _client.CreatePlayerAsync(
			apiUrl: command.Configuration.ApiUrl,
			agentId: command.Configuration.AgentId,
			agentKey: command.Configuration.AgentKey,
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken);
}
