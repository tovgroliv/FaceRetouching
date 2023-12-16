using FaceRetouching.PluginSystem;
using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App.Controls;

public partial class PluginControl : UserControl
{
	public IPlugin Plugin { get; set; }

	public string PluginName
	{
		get => (string)GetValue(PluginNameProperty);
		set => SetValue(PluginNameProperty, value);
	}

	public static readonly DependencyProperty PluginNameProperty = DependencyProperty.Register(
		nameof(PluginName),
		typeof(string),
		typeof(PluginControl),
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
		typeof(PluginControl),
		new FrameworkPropertyMetadata(
			string.Empty,
			FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
		)
	);

	public bool IsSelected => select.IsChecked ?? false;

	public PluginControl(IPlugin plugin) : base()
	{
		InitializeComponent();

		Plugin = plugin;
		PluginName = plugin.Name;
		PluginDescription = plugin.Description;
	}
}
