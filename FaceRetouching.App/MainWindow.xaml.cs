using FaceRetouching.App.Controls;
using FaceRetouching.PluginSystem;
using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		PluginLoader Loader { get; set; } = new();

		public MainWindow()
		{
			Loader.LoadPlugins();

			InitializeComponent();

			Loader.Plugins.ForEach(plugin => pluginsList.Children.Add(new PluginControl(plugin)));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
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

			LoadingPanel.Visibility = Visibility.Visible;

			var bitmap = selectImage.Image;

			progressBar.Maximum = plugins.Count;

			Task.Run(() =>
			{
				if (bitmap != null)
				{
					int i = 0;
					plugins.ForEach(plugin =>
					{
						Application.Current.Dispatcher.BeginInvoke(() =>
						{
							progressStatus.Content = $"{i}/{plugins.Count} {plugin.Name}";
							progressBar.Value = i;
						});

						bitmap = plugin.DoWork(bitmap);

						i++;

						Thread.Sleep(200);
					});

					Application.Current.Dispatcher.BeginInvoke(() =>
					{
						progressStatus.Content = $"{i}/{plugins.Count} Завершение";
						progressBar.Value = i;
					});
					Thread.Sleep(200);
				}

				Application.Current.Dispatcher.BeginInvoke(() =>
				{
					selectImage.Image = bitmap;

					LoadingPanel.Visibility = Visibility.Collapsed;
				});
			});
		}
	}
}