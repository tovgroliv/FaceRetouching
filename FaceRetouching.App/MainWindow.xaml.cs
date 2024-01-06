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
	public MainWindow()
	{
		var client = ClientConnection.Source.Value;
		client.Connect();

		InitializeComponent();

		imageModifyPage.SelectImagePage = selectImagePage;
		imageModifyPage.ProgressBarControl = progressBarControl;
	}
}
