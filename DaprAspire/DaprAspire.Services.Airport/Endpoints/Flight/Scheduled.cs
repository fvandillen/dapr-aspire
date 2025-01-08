using Dapr;
using DaprAspire.Domain.Events;
using DaprAspire.Services.Airport.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Airport.Endpoints.Flight;

public static class Scheduled
{
    public static IEndpointRouteBuilder MapFlightScheduled(this IEndpointRouteBuilder app)
    {
        app.MapPost("/flight/scheduled", Handle);
        return app;
    }

    [Topic("pubsub", "flight-scheduled")]
    private static IResult Handle(
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] AirportMetrics metrics,
        CloudEvent<FlightScheduledEvent> @event
        )
    {
        var logger = loggerFactory.CreateLogger(typeof(Scheduled).FullName!);
        
        logger.LogInformation("Received flight scheduled event with {AircraftType} to {Destination}", @event.Data.AircraftType, @event.Data.Destination);
        metrics.FlightScheduled();
        
        return Results.Ok();
    }
}