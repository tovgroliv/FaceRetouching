using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FaceRetouching.App.Controls;

public partial class SelectImage : UserControl
{
	public Bitmap? Image
	{
		get => (Bitmap?)GetValue(ImageProperty);
		set
		{
			SetValue(ImageProperty, value);

			if (value != null) ShowImage();
			else HideImage();
		}
	}

	public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
		nameof(Image),
		typeof(Bitmap),
		typeof(SelectImage),
		new FrameworkPropertyMetadata(
			null,
			FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
		)
	);

	public SelectImage() => InitializeComponent();

	private void Border_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			Image = Validate(files[0]);
		}
	}

	private void Select_Click(object sender, RoutedEventArgs e)
	{
		var openFileDialog = new OpenFileDialog();
		if (openFileDialog.ShowDialog() == true)
		{
			Image = Validate(openFileDialog.FileName);
		}
	}

	private void ShowImage()
	{
		selectPanel.Visibility = Visibility.Collapsed;
		showPanel.Visibility = Visibility.Visible;

		if (Image != null)
		{
			image.Source = null;
			image.Source = ConvertBitmap(Image);
		}
	}

	private void HideImage()
	{
		selectPanel.Visibility = Visibility.Visible;
		showPanel.Visibility = Visibility.Collapsed;
	}

	private Bitmap? Validate(string path)
	{
		if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
		{
			return new Bitmap(path);
		}

		return null;
	}

	private void Save_Click(object sender, RoutedEventArgs e)
	{
		
	}

	private void Cancel_Click(object sender, RoutedEventArgs e)
	{
		Image = null;
	}

	public BitmapImage ConvertBitmap(Bitmap bitmap)
	{
		MemoryStream ms = new MemoryStream();
		bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
		BitmapImage image = new BitmapImage();
		image.BeginInit();
		ms.Seek(0, SeekOrigin.Begin);
		image.StreamSource = ms;
		image.EndInit();

		return image;
	}
}
