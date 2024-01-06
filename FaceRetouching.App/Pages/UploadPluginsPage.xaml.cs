using FaceRetouching.PluginSystem;
using FaceRetouching.PluginSystem.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App.Pages;

public partial class UploadPluginsPage : UserControl
{
	public UploadPluginsPage()
	{
		InitializeComponent();
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
		catch (Exception ex)
		{
			MessageBox.Show($"Ошибка при загрузке - {ex.Message}");
		}
	}
}
