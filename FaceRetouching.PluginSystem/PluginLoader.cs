using System.Reflection;

namespace FaceRetouching.PluginSystem;

public class PluginLoader
{
	public List<IPlugin> Plugins { get; private set; } = new();

	public void LoadPlugins()
	{
		Plugins.Clear();

		if (Directory.Exists(Constants.FolderName))
		{
			Directory.GetDirectories(Constants.FolderName).ToList()
				.ForEach(pluginPath =>
				{
					Directory
						.GetFiles(pluginPath).ToList()
						.Where(file => file.Contains("FaceRetouching.Plugin.")).ToList()
						.Where(file => file.EndsWith(".dll")).ToList()
						.ForEach(file =>
						{
							Assembly.LoadFrom(Path.GetFullPath(file));

							//var dll = File.ReadAllBytes(file);
							//var pdbPath = file.Replace(".dll", ".pdb");

							//if (File.Exists(pdbPath))
							//{
							//	var pdb = File.ReadAllBytes(pdbPath);
							//	Assembly.Load(dll, pdb);
							//}
							//else
							//{
							//	Assembly.Load(dll);
							//}
						});
				});
		}

		var interfaceType = typeof(IPlugin);

		AppDomain.CurrentDomain
			.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)
			.ToList()
			.ForEach(type =>
			{
				var plugin = Activator.CreateInstance(type);

				if (plugin != null)
				{
					Plugins.Add((IPlugin)plugin);
				}
			});
	}
}
