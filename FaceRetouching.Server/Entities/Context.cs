using Microsoft.EntityFrameworkCore;

namespace FaceRetouching.Server.Entities;

public class Context : DbContext
{
	public DbSet<PluginEntity> PluginEntities { get; set; }

	public Context()
	{
		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Filename=server.db");
	}
}
