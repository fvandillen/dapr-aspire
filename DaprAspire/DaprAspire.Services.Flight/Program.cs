using DaprAspire.Services.Flight.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.UseServiceDefaults();

app.MapScheduleFlight();

app.Run();