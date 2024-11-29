using Microsoft.EntityFrameworkCore;

namespace DaprAspire.Services.RegulatoryInspector.Database;

public class RegulatoryInspectorContext(DbContextOptions<RegulatoryInspectorContext> options) : DbContext(options)
{
	public DbSet<Violation> Violations { get; set; }
}