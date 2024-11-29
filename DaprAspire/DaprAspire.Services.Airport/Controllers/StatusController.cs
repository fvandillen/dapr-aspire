using System.Diagnostics;
using Dapr.Client;
using DaprAspire.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Airport.Controllers;

[ApiController]
[Route("status")]
public class StatusController(DaprClient daprClient, ILogger<StatusController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetStatus()
    {
        logger.LogInformation("Received status check");
        var status = await daprClient.GetStateAsync<bool>("statestore", "status");
        return Ok(status);
    }

    [HttpPost("open")]
    public async Task<IActionResult> Open()
    {
        var currentStatus = await daprClient.GetStateAsync<bool>("statestore", "status");

        if (currentStatus is false)
        {
            logger.LogInformation("The airport has been opened ✈✈");
            await daprClient.SaveStateAsync("statestore", "status", true);
            await daprClient.PublishEventAsync("pubsub", "airport-status-changed", new AirportStatusChangedEvent(true, DateTime.Now));

            return Ok();
        }
        else
        {
            Activity.Current?.AddTag("airport.open.failure-reason", "The airport is already open.");
            return BadRequest("The airport is already open!");
        }
    }
    
    [HttpPost("close")]
    public async Task<IActionResult> Close()
    {
        var currentStatus = await daprClient.GetStateAsync<bool>("statestore", "status");

        if (currentStatus is true)
        {
            logger.LogInformation("The airport has been closed");
            await daprClient.SaveStateAsync("statestore", "status", false);
            await daprClient.PublishEventAsync("pubsub", "airport-status-changed", new AirportStatusChangedEvent(false, DateTime.Now));

            return Ok();
        }
        else
        {
            Activity.Current?.AddTag("airport.close.failure-reason", "The airport is already closed.");
            return BadRequest("The airport is already closed!");
        }
    }
}