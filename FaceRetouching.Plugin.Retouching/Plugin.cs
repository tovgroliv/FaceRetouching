using FaceRetouching.PluginSystem;
using System.Drawing;

#pragma warning disable CA1416

namespace FaceRetouching.Plugin.Retouching;

public class Plugin : IPlugin
{
	public string Name => "Ретуширование лица";
	public string Description => "Ретуширование лица";
	public Bitmap DoWork(Bitmap input)
	{
		var result = input;

		for (var i = 0; i < 10; i++)
		{
			for (var j = 0; j < 10; j++)
			{
				result.SetPixel(i, j, Color.Red);
			}
		}

		return result;
	}
}
