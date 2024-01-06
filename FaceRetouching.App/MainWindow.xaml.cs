using FaceRetouching.PluginSystem.Services;
using System.Windows;

namespace FaceRetouching.App;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		var client = ClientConnection.Source.Value;
		client.Connect();

		InitializeComponent();

		imageModifyPage.SelectImagePage = selectImagePage;
		imageModifyPage.ProgressBarControl = progressBarControl;
	}
}
