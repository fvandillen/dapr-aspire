using Dapr.Client;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Flight.Controllers;

[ApiController]
public class SchedulerController(DaprClient daprClient, ILogger<SchedulerController> logger) : ControllerBase
{
	[HttpPost("~/scheduler")]
	public async Task<IActionResult> ScheduleFlights()
	{
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
		
		return Ok();
	}
}