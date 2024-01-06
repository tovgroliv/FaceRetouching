using FaceRetouching.App.Controls;
using FaceRetouching.PluginSystem;
using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App.Pages;

public partial class ImageModifyPage : UserControl
{
	private PluginLoader Loader { get; set; } = new();

	public SelectImagePage? SelectImagePage { get; set; }
	public ProgressBarControl? ProgressBarControl { get; set; }

	public ImageModifyPage()
	{
		InitializeComponent();

		Loader.LoadPlugins();
		Loader.Plugins.ForEach(plugin => pluginsList.Children.Add(new PluginControl(plugin)));
	}

	private void Run_Click(object sender, RoutedEventArgs e)
	{
		var plugins = new List<IPlugin>();

		foreach (PluginControl? item in pluginsList.Children)
		{
			if (item != null && item.IsSelected)
			{
				plugins.Add(item.Plugin);
			}
		}

		if (plugins.Count == 0)
		{
			MessageBox.Show("Нужно выбрать хотя бы один плагин");
			return;
		}

		if (SelectImagePage == null || ProgressBarControl == null)
		{
			throw new Exception();
		}

		ProgressBarControl.Status = Visibility.Visible;

		var bitmap = SelectImagePage.Image;

		ProgressBarControl.MaxValue = plugins.Count;

		Task.Run(() =>
		{
			if (bitmap != null)
			{
				int i = 0;
				plugins.ForEach(plugin =>
				{
					Application.Current.Dispatcher.BeginInvoke(() =>
					{
						ProgressBarControl.Label = $"{i}/{plugins.Count} {plugin.Name}";
						ProgressBarControl.Value = i;
					});

					bitmap = plugin.DoWork(bitmap);

					i++;

					Thread.Sleep(200);
				});

				Application.Current.Dispatcher.BeginInvoke(() =>
				{
					ProgressBarControl.Label = $"{i}/{plugins.Count} Завершение";
					ProgressBarControl.Value = i;
				});
				Thread.Sleep(200);
			}

			Application.Current.Dispatcher.BeginInvoke(() =>
			{
				SelectImagePage.Image = bitmap;

				ProgressBarControl.Status = Visibility.Collapsed;
			});
		});
	}
}
