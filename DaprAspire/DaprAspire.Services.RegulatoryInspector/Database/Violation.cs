namespace DaprAspire.Services.RegulatoryInspector.Database;

public class Violation
{
	public Guid Id { get; set; }
	public DateTime DateTime { get; set; }
	public string? Description { get; set; }
}