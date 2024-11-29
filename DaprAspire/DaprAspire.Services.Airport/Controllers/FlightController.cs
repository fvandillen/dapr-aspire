using Dapr;
using Dapr.Client;
using DaprAspire.Services.Airport.Metrics;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Airport.Controllers;

[ApiController]
[Route("flight")]
public class FlightController(AirportMetrics metrics, ILogger<FlightController> logger) : ControllerBase
{
	[Topic("pubsub", "flight-scheduled")]
	[HttpPost("scheduled")]
	public IActionResult FlightScheduled(CloudEvent<FlightScheduledEvent> @event)
	{
		logger.LogInformation("Received flight scheduled event with {AircraftType} to {Destination}", @event.Data.AircraftType, @event.Data.Destination);
		metrics.FlightScheduled();
		return Ok();
	}
}