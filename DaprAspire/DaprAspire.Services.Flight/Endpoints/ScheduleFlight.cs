using Dapr.Client;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Flight.Endpoints;

public static class ScheduleFlight
{
	public static IEndpointRouteBuilder MapScheduleFlight(this IEndpointRouteBuilder app)
	{
		app.MapPost("/scheduler", Handle);
		return app;
	}
	
	private static async Task<IResult> Handle(
		[FromServices] ILoggerFactory loggerFactory,
		[FromServices] DaprClient daprClient)
	{
		var logger = loggerFactory.CreateLogger(typeof(ScheduleFlight).FullName!);
		
		logger.LogInformation("Attempting to schedule flights, checking if airport is open");
		// First, check if the airport is open.
		var request = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "airport", "status");
		var response = await daprClient.InvokeMethodAsync<bool>(request);

		if (response)
		{
			logger.LogInformation("Airport is open. Scheduling flights");
			
			var aircraftTypes = new[] { "Boeing 747", "Airbus A380", "Boeing 737" };
			var destinations = new[] { "London", "Paris", "New York", "Tokyo" };
			var randomAircraftType = aircraftTypes[new Random().Next(aircraftTypes.Length)];
			var randomDestination = destinations[new Random().Next(destinations.Length)];
			
			var flight = new FlightScheduledEvent(DateTime.UtcNow, randomAircraftType, randomDestination);
			await daprClient.PublishEventAsync("pubsub", "flight-scheduled", flight);
			
			logger.LogInformation("Scheduled flight with {AirCraftType} to {Destination}", flight.AircraftType, flight.Destination);
		}
		else
		{
			logger.LogWarning("Airport is closed. Not scheduling flights");
		}
		
		return Results.Ok();
	}
}