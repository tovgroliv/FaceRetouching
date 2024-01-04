using Google.Protobuf;
using Grpc.Net.Client;
using System.IO;
using System;

namespace FaceRetouching.PluginSystem.Services;

internal class PluginsService : IService
{
	private Plugins.PluginsClient? Service { get; set; }

	public bool Connect(GrpcChannel Channel)
	{
		Service = new(Channel);

		return Service != null;
	}

	public async Task<UploadReply> Upload(string name, string description, string path, byte[] lib)
	{
		if (Service == null) throw new Exception();

		var bs = ByteString.CopyFrom(lib);

		return await Service.UploadAsync(new() { Name = name, Description = description, Path = path, Lib = bs });
	}

	public async Task<UploadReply> Upload(string guid, string path, byte[] lib)
	{
		if (Service == null) throw new Exception();

		var bs = ByteString.CopyFrom(lib);

		return await Service.UploadAsync(new() { Guid = guid, Path = path, Lib = bs });
	}

	public async Task<ListReply> GetList()
	{
		if (Service == null) throw new Exception();

		return await Service.ListAsync(new());
	}

	public async Task<LoadReply> Load(Guid guid)
	{
		if (Service == null) throw new Exception();

		return await Service.LoadAsync(new() { Guid = guid.ToString() });
	}
}
