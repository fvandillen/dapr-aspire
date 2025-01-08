using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace DaprAspire.Services.Airport.Endpoints.Status;

public static class Get
{
    public static IEndpointRouteBuilder MapStatusGet(this IEndpointRouteBuilder app)
    {
        app.MapGet("/status", Handle);
        return app;
    }
    
    private static async Task<IResult> Handle(
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] DaprClient daprClient)
    {
        var logger = loggerFactory.CreateLogger(typeof(Get).FullName!);
        
        logger.LogInformation("Received status check");
        var status = await daprClient.GetStateAsync<bool>("statestore", "status");
        
        return Results.Ok(status);
    }
}