using FaceRetouching.App.Controls;
using FaceRetouching.PluginSystem;
using FaceRetouching.PluginSystem.Entities;
using FaceRetouching.PluginSystem.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App.Pages;

public partial class LoadPlugins : UserControl
{
	public LoadPlugins()
	{
		InitializeComponent();
	}

	private async void UpdateList_Click(object sender, RoutedEventArgs e)
	{
		remotePlugins.Children.Clear();

		try
		{
			var client = ClientConnection.Source.Value;

			ListReply reply = await client.PluginsService.GetList();

			reply.Plugins.ToList()
				.ForEach(x =>
				{
					using (var db = new Context())
					{
						var plugin = db.PluginEntities.FirstOrDefault(plugin => plugin.Id == Guid.Parse(x.Guid));

						var child = new PluginUploadControl(x.Guid, x.Name, x.Description, x.LastUpdate.ToDateTime(), plugin != null, plugin?.LastUpdate != x.LastUpdate.ToDateTime());
						remotePlugins.Children.Add(child);
					}
				});
		}
		catch (Exception ex) when (!Debugger.IsAttached)
		{
			MessageBox.Show($"Ошибка при загрузке - {ex.Message}");
		}
	}
}
