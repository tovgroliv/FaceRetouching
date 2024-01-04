using FaceRetouching.PluginSystem.Entities;
using Google.Protobuf;
using Grpc.Net.Client;
using System.IO.Compression;

namespace FaceRetouching.PluginSystem.Services;

public class PluginsService : IService
{
	private Plugins.PluginsClient? Service { get; set; }

	public bool Connect(GrpcChannel Channel)
	{
		Service = new(Channel);

		return Service != null;
	}

	public async Task<UploadReply> Upload(string name, string description, byte[] lib)
	{
		if (Service == null) throw new Exception();

		var bs = ByteString.CopyFrom(lib);

		return await Service.UploadAsync(new() { Name = name, Description = description, Lib = bs });
	}

	public async Task<UploadReply> Upload(string guid, string name, string description, byte[] lib)
	{
		if (Service == null) throw new Exception();

		var bs = ByteString.CopyFrom(lib);

		return await Service.UploadAsync(new() { Guid = guid, Name = name, Description = description, Lib = bs });
	}

	public async Task<ListReply> GetList()
	{
		if (Service == null) throw new Exception();

		return await Service.ListAsync(new());
	}

	public async Task Load(Guid guid)
	{
		if (Service == null) throw new Exception();

		if (!Directory.Exists("Temp"))
		{
			Directory.CreateDirectory("Temp");
		}

		var result = await Service.LoadAsync(new() { Guid = guid.ToString() });

		using (var db = new Context())
		{
			var plugin = db.PluginEntities.FirstOrDefault(x => x.Id == Guid.Parse(result.Plugin.Guid));

			if (plugin != null)
			{
				plugin.Name = result.Plugin.Name;
				plugin.Description = result.Plugin.Description;
				plugin.LastUpdate = result.Plugin.LastUpdate.ToDateTime();

				db.PluginEntities.Update(plugin);
				db.SaveChanges();

				File.WriteAllBytes($"Temp/{plugin.Id}", result.Plugin.Lib.ToArray());

				ZipFile.ExtractToDirectory($"Temp/{plugin.Id}", $"Plugins/{plugin.Id}");
			}
			else
			{
				plugin = new()
				{
					Id = Guid.Parse(result.Plugin.Guid),
					Name = result.Plugin.Name,
					Description = result.Plugin.Description,
					LastUpdate = result.Plugin.LastUpdate.ToDateTime()
				};

				db.PluginEntities.Add(plugin);
				db.SaveChanges();

				File.WriteAllBytes($"Temp/{plugin.Id}", result.Plugin.Lib.ToArray());

				ZipFile.ExtractToDirectory($"Temp/{plugin.Id}", $"Plugins/{plugin.Id}");
			}
		}
	}

	public void Remove(Guid guid)
	{
		if (!Directory.Exists("Temp"))
		{
			Directory.CreateDirectory("Temp");
		}

		using (var db = new Context())
		{
			var plugin = db.PluginEntities.FirstOrDefault(x => x.Id == guid);

			if (plugin == null)
			{
				throw new Exception("Plugin not found");
			}

			db.PluginEntities.Remove(plugin);
			db.SaveChanges();

			File.Delete($"Temp/{plugin.Id}");

			Directory.Delete($"Plugins/{plugin.Id}", true);
		}
	}
}
