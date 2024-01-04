using FaceRetouching.PluginSystem;
using FaceRetouching.PluginSystem.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App.Controls;

public partial class PluginUploadControl : UserControl
{
	public string PluginGuid
	{
		get => (string)GetValue(PluginGuidProperty);
		set => SetValue(PluginGuidProperty, value);
	}

	public static readonly DependencyProperty PluginGuidProperty = DependencyProperty.Register(
		nameof(PluginGuid),
		typeof(string),
		typeof(PluginUploadControl),
		new FrameworkPropertyMetadata(
			string.Empty,
			FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
		)
	);

	public string PluginName
	{
		get => (string)GetValue(PluginNameProperty);
		set => SetValue(PluginNameProperty, value);
	}

	public static readonly DependencyProperty PluginNameProperty = DependencyProperty.Register(
		nameof(PluginName),
		typeof(string),
		typeof(PluginUploadControl),
		new FrameworkPropertyMetadata(
			string.Empty,
			FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
		)
	);

	public string PluginDescription
	{
		get => (string)GetValue(PluginDescriptionProperty);
		set => SetValue(PluginDescriptionProperty, value);
	}

	public static readonly DependencyProperty PluginDescriptionProperty = DependencyProperty.Register(
		nameof(PluginDescription),
		typeof(string),
		typeof(PluginUploadControl),
		new FrameworkPropertyMetadata(
			string.Empty,
			FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
		)
	);

	public DateTime PluginLastUpdate
	{
		get => (DateTime)GetValue(PluginLastUpdateProperty);
		set => SetValue(PluginLastUpdateProperty, value);
	}

	public static readonly DependencyProperty PluginLastUpdateProperty = DependencyProperty.Register(
		nameof(PluginLastUpdate),
		typeof(DateTime),
		typeof(PluginUploadControl),
		new FrameworkPropertyMetadata(
			DateTime.UtcNow,
			FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
		)
	);

	public PluginUploadControl(string guid, string name, string description, DateTime lastUpdate, bool loaded, bool needUpdate) : base()
	{
		InitializeComponent();

		PluginGuid = guid;
		PluginName = name;
		PluginDescription = description;
		PluginLastUpdate = lastUpdate;

		load.Content = loaded ? "Обновить" : "Загрузить";
		load.IsEnabled = needUpdate || !loaded;

		remove.Visibility = loaded ? Visibility.Visible : Visibility.Collapsed;
	}

	private async void Load_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			var client = ClientConnection.Source.Value;

			await client.PluginsService.Load(Guid.Parse(PluginGuid));
		}
		catch (Exception ex) when (!Debugger.IsAttached)
		{
			MessageBox.Show($"Ошибка при загрузке - {ex.Message}");
		}
	}

	private void Remove_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			var client = ClientConnection.Source.Value;

			client.PluginsService.Remove(Guid.Parse(PluginGuid));
		}
		catch (Exception ex) when (!Debugger.IsAttached)
		{
			MessageBox.Show($"Ошибка при удалении - {ex.Message}");
		}
	}
}
