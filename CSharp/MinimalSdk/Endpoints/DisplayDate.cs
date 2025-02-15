namespace MinimalSdk.Endpoints;

using MinimalSdk.Slices;
using RazorSlices;
using StarFederation.Datastar.DependencyInjection;

public static class DisplayDateEndpoints
{
    public static void MapDisplayDate(this WebApplication app)
    {
        app.MapGet(
            "/displayDate",
            async (IDatastarServerSentEventService sse) =>
            {
                var slice = DisplayDate.Create(DateTime.Now);
                var fragment = await slice.RenderAsync();
                await sse.MergeFragmentsAsync(fragment);
            }
        );

        app.MapDelete(
            "/displayDate",
            async (IDatastarServerSentEventService sse) =>
            {
                await sse.RemoveFragmentsAsync("#date");
            }
        );
    }
}
