namespace MinimalSdk.Endpoints;

using MinimalSdk.Models;
using MinimalSdk.Slices;
using StarFederation.Datastar.DependencyInjection;

public static class QuizEndpoints
{
    public static void MapQuiz(this WebApplication app)
    {
        app.MapGet("/quiz", () => Results.Extensions.RazorSlice<Quiz>());

        app.MapPost(
            "/quiz",
            async (IDatastarServerSentEventService sse) =>
            {
                await Task.Delay(1000);
                await sse.MergeFragmentsAsync(
                    """<div id="question">What do you put in a toaster?</div>"""
                );

                var newSignals = new QuizSignals { Answer = "bread" };
                await sse.MergeSignalsAsync(newSignals.Serialize());
            }
        );
    }
}
