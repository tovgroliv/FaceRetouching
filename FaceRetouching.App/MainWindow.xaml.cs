using FaceRetouching.App.Controls;
using FaceRetouching.PluginSystem;
using System.Windows;

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

			var bitmap = selectImage.Image;

			if (bitmap != null)
			{
				plugins.ForEach(plugin =>
				{
					bitmap = plugin.DoWork(bitmap);
				});
			}

			selectImage.Image = bitmap;
			var pixel = bitmap.GetPixel(0, 0);
        }
	}
}