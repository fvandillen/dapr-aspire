using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.RegulatoryInspector.Controllers;

[ApiController]
[Route("violations")]
public class ViolationsController(DaprClient daprClient, ILogger<ViolationsController> logger) : ControllerBase
{
	[HttpPost("clear")]
	public async Task<IActionResult> ClearViolations()
	{
		logger.LogInformation("Resetting the big airplane count to 0");
		await daprClient.SaveStateAsync("statestore", "big-airplane-count", 0);

		return Ok();
	}
}