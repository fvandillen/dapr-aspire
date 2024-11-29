namespace DaprAspire.AppHost.Extensions;

internal static class AirportServiceResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> WithOpenAirportCommand(
        this IResourceBuilder<ProjectResource> builder)
    {
        builder.WithCommand(
            name: "airport-open",
            displayName: "Open airport",
            executeCommand: context => OnOpenAirportCommandAsync(builder, context),
            iconName: "AirplaneTakeOff",
            iconVariant: IconVariant.Filled
        );

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnOpenAirportCommandAsync(IResourceBuilder<ProjectResource> builder, ExecuteCommandContext context)
    {
        // Get the URL of the resource.
        var endpoint = builder.GetEndpoint("https").Url;
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(endpoint);
        
        var response = await httpClient.PostAsync("status/open", new StringContent(string.Empty));
        if (!response.IsSuccessStatusCode)
            return new ExecuteCommandResult()
                { ErrorMessage = "Unable to open the airport (was it already opened?)", Success = false };
        
        return CommandResults.Success();
    }
    
    public static IResourceBuilder<ProjectResource> WithCloseAirportCommand(
        this IResourceBuilder<ProjectResource> builder)
    {
        builder.WithCommand(
            name: "airport-close",
            displayName: "Close airport",
            executeCommand: context => OnCloseAirportCommandAsync(builder, context),
            iconName: "AirplaneLanding",
            iconVariant: IconVariant.Filled
        );

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnCloseAirportCommandAsync(IResourceBuilder<ProjectResource> builder, ExecuteCommandContext context)
    {
        // Get the URL of the resource.
        var endpoint = builder.GetEndpoint("https").Url;
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(endpoint);
        
        var response = await httpClient.PostAsync("status/close", new StringContent(string.Empty));
        if (!response.IsSuccessStatusCode)
            return new ExecuteCommandResult()
                { ErrorMessage = "Unable to close the airport (was it already closed?)", Success = false };
        
        return CommandResults.Success();
    }
}