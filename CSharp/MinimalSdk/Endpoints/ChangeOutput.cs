namespace MinimalSdk.Endpoints;

using MinimalSdk.Models;
using StarFederation.Datastar.DependencyInjection;

public static class ChangeOutputEndpoints
{
    public static void MapChangeOutput(this WebApplication app)
    {
        app.MapPost(
            "/changeOutput",
            async (IDatastarServerSentEventService sse, IDatastarSignalsReaderService dsSignals) =>
            {
                var signals = await dsSignals.ReadSignalsAsync<HomeSignals>();
                var newSignals = new HomeSignals { Output = $"Your input: {signals.Input}" };
                await sse.MergeSignalsAsync(newSignals.Serialize());
            }
        );
    }
}
