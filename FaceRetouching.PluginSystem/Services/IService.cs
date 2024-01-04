using Grpc.Net.Client;

namespace FaceRetouching.PluginSystem.Services;
public interface IService
{
	bool Connect(GrpcChannel Channel);
}
