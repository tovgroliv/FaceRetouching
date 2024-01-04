using Grpc.Net.Client;

namespace FaceRetouching.PluginSystem.Services;

public class ClientConnection
{
	private GrpcChannel? Channel { get; set; }

	public bool IsConnected => Channel?.State == Grpc.Core.ConnectivityState.Ready;

	public delegate void ConnectedHandler(object sender);
	public event ConnectedHandler? Connected;

	public delegate void ConnectionClosedHandler(object sender);
	public event ConnectionClosedHandler? ConnectionClosed;

	public PluginsService PluginsService { get; private set; } = new();

	public async Task Connect()
	{
		var thread = new Thread(ClientThread);
		thread.IsBackground = true;
		thread.Start();

		Channel = GrpcChannel.ForAddress("http://localhost:5000/");

		await Channel.ConnectAsync();
	}

	private async void ClientThread()
	{
		while (true)
		{
			if (Channel == null)
			{
				continue;
			}

			await Channel.WaitForStateChangedAsync(Channel.State);

			if (Channel.State == Grpc.Core.ConnectivityState.Ready)
			{
				PluginsService.Connect(Channel);
			}
			if (Channel.State == Grpc.Core.ConnectivityState.Idle)
			{
				await Channel.ConnectAsync();
			}
		}
	}

	private static Lazy<ClientConnection> _value = new Lazy<ClientConnection>();
	public static Lazy<ClientConnection> Source => _value;
}
