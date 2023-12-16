using FaceRetouching.PluginSystem;
using System.Drawing;

#pragma warning disable CA1416

namespace FaceRetouching.Plugin.Retouching;

public class Plugin : IPlugin
{
	public string Name => "Красный глаз";
	public string Description => "Убрать с изображения красный глаз";
	public Bitmap DoWork(Bitmap input)
	{
		var result = input;

		for (var i = input.Width - 10; i < input.Width; i++)
		{
			for (var j = input.Height - 10; j < input.Height; j++)
			{
				result.SetPixel(i, j, Color.Blue);
			}
		}

		return result;
	}
}
