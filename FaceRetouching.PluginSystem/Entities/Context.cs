using Microsoft.EntityFrameworkCore;

namespace FaceRetouching.PluginSystem.Entities;

public class Context : DbContext
{
	public DbSet<PluginEntity> PluginEntities { get; set; }

	public Context()
	{
		if (!File.Exists("local.db"))
		{
			File.Create("local.db");
		}

		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Filename=local.db");
	}
}
