using Dapr;
using Dapr.Client;
using DaprAspire.Services.RegulatoryInspector.Database;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.RegulatoryInspector.Controllers;

[ApiController]
[Route("big-airplane")]
public class BigAirplaneController(DaprClient daprClient, RegulatoryInspectorContext db, ILogger<BigAirplaneController> logger) : ControllerBase
{
	[Topic("pubsub", "flight-scheduled", "event.data.aircraftType == \"Boeing 747\" || event.data.aircraftType == \"Airbus A380\"", 1)]
	[HttpPost("scheduled")]
	public async Task<IActionResult> BigAirplaneScheduled(CloudEvent<FlightScheduledEvent> @event)
	{
		logger.LogInformation("Received flight scheduled event with {AircraftType} to {Destination}", @event.Data.AircraftType, @event.Data.Destination);
		
		var bigAirplaneCount = await daprClient.GetStateAsync<int>("statestore", "big-airplane-count");
		bigAirplaneCount++;

		if (bigAirplaneCount > 5)
		{
			logger.LogWarning("Violation detected: More than 5 big airplanes scheduled");
			db.Violations.Add(new Violation()
			{
				DateTime = DateTime.Now, 
				Description = "Violation detected: More than 5 big airplanes scheduled"
			});
			await db.SaveChangesAsync();
		}
		
		await daprClient.SaveStateAsync("statestore", "big-airplane-count", bigAirplaneCount);
		
		return Ok();
	}
}