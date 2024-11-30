using DaprAspire.Constants;
using DaprAspire.Services.RegulatoryInspector.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<RegulatoryInspectorContext>(ResourceNames.RegulatoryInspectorSqlDatabase);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<RegulatoryInspectorContext>();
	context.Database.EnsureCreated();
}
app.UseServiceDefaults();

app.Run();