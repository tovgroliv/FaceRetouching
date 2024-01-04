using FaceRetouching.App.Controls;
using FaceRetouching.PluginSystem;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using FaceRetouching.PluginSystem.Services;
using System.Diagnostics;
using FaceRetouching.PluginSystem.Entities;

namespace FaceRetouching.App;

public partial class MainWindow : Window
{
	private PluginLoader Loader { get; set; } = new();

	public MainWindow()
	{
		var client = ClientConnection.Source.Value;
		client.Connect();

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

	private void SelectPlugin_Click(object sender, RoutedEventArgs e)
	{
		var openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Archive (*.zip)|*.zip";

		if (openFileDialog.ShowDialog() == true)
		{
			pluginPath.Text = openFileDialog.FileName;
		}
	}

	private async void Upload_Click(object sender, RoutedEventArgs e)
	{
		if (pluginPath.Text == string.Empty || !File.Exists(pluginPath.Text))
		{
			MessageBox.Show($"Файл не найден");
			return;
		}

		if (pluginDescription.Text == string.Empty || pluginName.Text == string.Empty)
		{
			MessageBox.Show($"Имя и описание обязательно для ввода");
			return;
		}

		var lib = File.ReadAllBytes(pluginPath.Text);

		try
		{
			var client = ClientConnection.Source.Value;

			UploadReply reply;

			if (pluginGuid.Text != string.Empty)
			{
				reply = await client.PluginsService.Upload(pluginGuid.Text, pluginName.Text, pluginDescription.Text, lib);
			}
			else
			{
				reply = await client.PluginsService.Upload(pluginName.Text, pluginDescription.Text, lib);
			}

			if (reply.Success)
			{
				MessageBox.Show($"Плагин успешно загружен - {reply.Guid}");
			}
			else
			{
				MessageBox.Show("Ошибка при загрузке");
			}
		}
		catch(Exception ex)
		{
			MessageBox.Show($"Ошибка при загрузке - {ex.Message}");
		}
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
