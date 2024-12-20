using System.Collections.Immutable;
using Aspire.Hosting.Dapr;
using DaprAspire.AppHost.Extensions;
using DaprAspire.Constants;

var builder = DistributedApplication.CreateBuilder(args);

#region Dapr

builder.AddDapr();
var daprComponentDirectory = ImmutableHashSet.Create(Directory.GetCurrentDirectory() + "/../dapr/components");

#endregion

#region CosmosDB

var cosmosDb = builder.AddContainer(ResourceNames.CosmosDb, "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator")
    .WithImageTag("vnext-preview")
    .WithEnvironment("PROTOCOL", "https")
    .WithHttpsEndpoint(8081, 8010, "gateway")
    .WithHttpsEndpoint(1234, 1234, "data-explorer");

// builder.Eventing.Subscribe<ResourceReadyEvent>(cosmosDb.Resource, (@event, token) =>
// {
//     Console.WriteLine($"Event: {@event}");
//     return Task.CompletedTask;
// });

#endregion

#region Airport service

builder
    .AddProject<Projects.DaprAspire_Services_Airport>(ResourceNames.AirportService)
    .WithOpenAirportCommand()
    .WithCloseAirportCommand()
    .WithDaprSidecar(new DaprSidecarOptions()
    {
        ResourcesPaths = daprComponentDirectory
    });

#endregion

#region Regulatory inspector service



var sql = builder
    .AddSqlServer(ResourceNames.RegulatoryInspectorSql)
    .AddDatabase(ResourceNames.RegulatoryInspectorSqlDatabase);



builder
    .AddProject<Projects.DaprAspire_Services_RegulatoryInspector>(ResourceNames.RegulatoryInspectorService)
    .WaitFor(sql)
    .WithReference(sql)
    .WithDaprSidecar(new DaprSidecarOptions()
    {
        ResourcesPaths = daprComponentDirectory
    });

#endregion

#region Flight service

builder
    .AddProject<Projects.DaprAspire_Services_Flight>(ResourceNames.FlightService)
    .WithDaprSidecar(new DaprSidecarOptions()
    {
        ResourcesPaths = daprComponentDirectory
    });

#endregion

builder.Build().Run();