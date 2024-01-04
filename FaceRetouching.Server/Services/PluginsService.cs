using FaceRetouching.Server.Entities;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FaceRetouching.Server.Services;

public class PluginsService : Plugins.PluginsBase
{
	private readonly ILogger<PluginsService> _logger;

	public PluginsService(ILogger<PluginsService> logger)
	{
		_logger = logger;
	}

	public override Task<ListReply> List(ListRequest request, ServerCallContext context)
	{
		var con = new Context();

		var listReply = new ListReply();

		using (var db = new Context())
		{
			listReply.Plugins.AddRange(db.PluginEntities
				.Select(x => new PluginInfo()
				{
					Guid = x.Id.ToString(),
					Name = x.Name,
					Description = x.Description,
					LastUpdate = DateTime.SpecifyKind(x.LastUpdate, DateTimeKind.Utc).ToTimestamp()
				})
				.ToList());
		}

		return Task.FromResult(listReply);
	}

	public override Task<LoadReply> Load(LoadRequest request, ServerCallContext context)
	{
		var con = new Context();

		var loadReply = new LoadReply() { Plugin = new() };

		using (var db = new Context())
		{
			var plugin = db.PluginEntities.FirstOrDefault(x => x.Id == Guid.Parse(request.Guid));

			if (plugin != null)
			{
				loadReply.Plugin.Guid = plugin.Id.ToString();
				loadReply.Plugin.Name = plugin.Name;
				loadReply.Plugin.Description = plugin.Description;
				loadReply.Plugin.LastUpdate = DateTime.SpecifyKind(plugin.LastUpdate, DateTimeKind.Utc).ToTimestamp();
				loadReply.Plugin.Lib = ByteString.CopyFrom(RetrievePlugin(loadReply.Plugin.Guid));
			}
		}

		return Task.FromResult(loadReply);
	}

	public override Task<UploadReply> Upload(UploadRequest request, ServerCallContext context)
	{
		var con = new Context();

		var uploadReply = new UploadReply();
		PluginEntity plugin;

		if (request.Guid == "")
		{
			using (var db = new Context())
			{
				plugin = new PluginEntity() { Name = request.Name, Description = request.Description };

				db.PluginEntities.Add(plugin);
				db.SaveChanges();

				SavePlugin(plugin.Id.ToString(), request.Lib.ToArray());
			}
		}
		else
		{
			using (var db = new Context())
			{
				plugin = db.PluginEntities.FirstOrDefault(x => x.Id == Guid.Parse(request.Guid));

				if (plugin != null)
				{
					plugin.LastUpdate = DateTime.UtcNow;
					db.PluginEntities.Update(plugin);
					db.SaveChanges();

					SavePlugin(plugin.Id.ToString(), request.Lib.ToArray());
				}
			}
		}

		uploadReply.Guid = plugin?.Id.ToString() ?? "";
		uploadReply.Success = plugin != null;

		return Task.FromResult(uploadReply);
	}

	private void SavePlugin(string guid, byte[] lib)
	{
		if (!Directory.Exists("Plugins"))
		{
			Directory.CreateDirectory("Plugins");
		}

		var filePath = $"Plugins/{guid}";

		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}

		File.WriteAllBytes(filePath, lib);
	}

	private byte[] RetrievePlugin(string guid)
	{
		if (!Directory.Exists("Plugins"))
		{
			Directory.CreateDirectory("Plugins");
		}

		var filePath = $"Plugins/{guid}";
		var plugin = File.ReadAllBytes(filePath);

		return plugin;
	}
}
