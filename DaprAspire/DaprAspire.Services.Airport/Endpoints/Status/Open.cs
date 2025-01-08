using System.Diagnostics;
using Dapr.Client;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Airport.Endpoints.Status;

public static class Open
{
    public static IEndpointRouteBuilder MapStatusOpen(this IEndpointRouteBuilder app)
    {
        app.MapPost("/status/open", Handle);
        return app;
    }
    
    private static async Task<IResult> Handle(
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] DaprClient daprClient)
    {
        var logger = loggerFactory.CreateLogger(typeof(Open).FullName!);
        
        var currentStatus = await daprClient.GetStateAsync<bool>("statestore", "status");

        if (currentStatus is false)
        {
            logger.LogInformation("The airport has been opened ✈✈");
            await daprClient.SaveStateAsync("statestore", "status", true);
            await daprClient.PublishEventAsync("pubsub", "airport-status-changed", new AirportStatusChangedEvent(true, DateTime.Now));

            return Results.Ok();
        }
        else
        {
            Activity.Current?.AddTag("airport.open.failure-reason", "The airport is already open.");
            return Results.BadRequest("The airport is already open!");
        }
    }
}