using System.Reflection;

namespace FaceRetouching.PluginSystem;

public class PluginLoader
{
	public static List<IPlugin> Plugins { get; private set; } = new();

	public void LoadPlugins()
	{
		Plugins.Clear();

		if (Directory.Exists(Constants.FolderName))
		{
			var files = Directory.GetFiles(Constants.FolderName).ToList();

			files
				.Where(file => file.EndsWith(".dll"))
				.ToList()
				.ForEach(file => Assembly.LoadFile(Path.GetFullPath(file)));
		}

		Type interfaceType = typeof(IPlugin);

		AppDomain.CurrentDomain.GetAssemblies()
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
