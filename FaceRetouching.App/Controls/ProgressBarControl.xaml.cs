using System.Windows;
using System.Windows.Controls;

namespace FaceRetouching.App.Controls;

public partial class ProgressBarControl : UserControl
{
	public double Value
	{
		get => progressBar.Value;
		set => progressBar.Value = value;
	}

	public double MaxValue
	{
		get => progressBar.Maximum;
		set => progressBar.Maximum = value;
	}

	public string Label
	{
		get => (string)progressStatus.Content;
		set => progressStatus.Content = value;
	}

	public Visibility Status
	{
		get => Visibility;
		set => Visibility = value;
	}

	public ProgressBarControl()
	{
		InitializeComponent();
	}
}
