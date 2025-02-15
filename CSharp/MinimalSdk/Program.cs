using System.Runtime.CompilerServices;
using MinimalSdk.Models;
using MinimalSdk.Slices;
using RazorSlices;
using StarFederation.Datastar.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebEncoders();
builder.Services.AddDatastar();

var app = builder.Build();

app.UseStatusCodePages();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => Results.Extensions.RazorSlice<Home>());

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

app.MapPost(
    "/changeOutput",
    async (IDatastarServerSentEventService sse, IDatastarSignalsReaderService dsSignals) =>
    {
        var signals = await dsSignals.ReadSignalsAsync<HomeSignals>();
        var newSignals = new HomeSignals { Output = $"Your input: {signals.Input}" };
        await sse.MergeSignalsAsync(newSignals.Serialize());
    }
);

app.MapGet("/quiz", () => Results.Extensions.RazorSlice<Quiz>());

app.MapPost(
    "/quiz",
    async (IDatastarServerSentEventService sse) =>
    {
        await Task.Delay(1000);
        await sse.MergeFragmentsAsync("""<div id="question">What do you put in a toaster?</div>""");

        var newSignals = new QuizSignals { Answer = "bread" };
        await sse.MergeSignalsAsync(newSignals.Serialize());
    }
);

Console.WriteLine($"ContentRoot Path: {builder.Environment.ContentRootPath}");
Console.WriteLine($"WebRoot Path: {builder.Environment.WebRootPath}");
Console.WriteLine(
    $"RuntimeFeature.IsDynamicCodeSupported = {RuntimeFeature.IsDynamicCodeSupported}"
);
Console.WriteLine($"RuntimeFeature.IsDynamicCodeCompiled = {RuntimeFeature.IsDynamicCodeCompiled}");

app.Run();
