using Grpc.Core;

namespace FaceRetouching.Server.Services;

public class PluginsService : Plugins.PluginsBase
{
	public override Task<ListReply> List(ListRequest request, ServerCallContext context)
	{
		return base.List(request, context);
	}

	public override Task<LoadReply> Load(LoadRequest request, ServerCallContext context)
	{
		return base.Load(request, context);
	}

	public override Task<UploadReply> Upload(UploadRequest request, ServerCallContext context)
	{
		return base.Upload(request, context);
	}
}
