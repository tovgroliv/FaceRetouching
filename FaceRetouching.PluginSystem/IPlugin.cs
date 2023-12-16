using System.Drawing;

namespace FaceRetouching.PluginSystem;

public interface IPlugin
{
	string Name { get; }
	string Description { get; }
	Bitmap DoWork(Bitmap input);
}
