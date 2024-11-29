using Microsoft.EntityFrameworkCore;

namespace DaprAspire.Services.RegulatoryInspector.Database;

public class RegulatoryInspectorContext : DbContext
{
	public DbSet<Violation> Violations { get; set; }

	public RegulatoryInspectorContext(DbContextOptions<RegulatoryInspectorContext> options) : base(options)
	{
		
	}
}