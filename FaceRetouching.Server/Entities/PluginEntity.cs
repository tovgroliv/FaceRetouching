namespace FaceRetouching.Server.Entities;

public class PluginEntity
{
	public Guid Id { get; set; }
	public string Name { get; set; } = "";
	public string Description { get; set; } = "";
	public DateTime LastUpdate { get; set; } = DateTime.Now;
}
