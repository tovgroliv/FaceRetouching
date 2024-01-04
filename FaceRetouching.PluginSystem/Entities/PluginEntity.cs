namespace FaceRetouching.PluginSystem.Entities;

public class PluginEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = "";
	public string Description { get; set; } = "";
	public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}
