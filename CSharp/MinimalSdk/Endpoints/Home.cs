namespace MinimalSdk.Endpoints;

using MinimalSdk.Slices;

public static class HomeEndpoints
{
    public static void MapHome(this WebApplication app)
    {
        app.MapGet("/", () => Results.Extensions.RazorSlice<Home>());
    }
}
