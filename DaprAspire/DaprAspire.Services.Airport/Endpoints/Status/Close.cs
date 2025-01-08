using System.Diagnostics;
using Dapr.Client;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Airport.Endpoints.Status;

public static class Close
{
    public static IEndpointRouteBuilder MapStatusClose(this IEndpointRouteBuilder app)
    {
        app.MapPost("/status/close", Handle);
        return app;
    }
    
    private static async Task<IResult> Handle(
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] DaprClient daprClient)
    {
        var logger = loggerFactory.CreateLogger(typeof(Close).FullName!);
        
        var currentStatus = await daprClient.GetStateAsync<bool>("statestore", "status");

        if (currentStatus is true)
        {
            logger.LogInformation("The airport has been closed");
            await daprClient.SaveStateAsync("statestore", "status", false);
            await daprClient.PublishEventAsync("pubsub", "airport-status-changed", new AirportStatusChangedEvent(false, DateTime.Now));

            return Results.Ok();
        }
        else
        {
            Activity.Current?.AddTag("airport.close.failure-reason", "The airport is already closed.");
            return Results.BadRequest("The airport is already closed!");
        }
    }
}