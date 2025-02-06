using System.Runtime.CompilerServices;
using StarFederation.Datastar.DependencyInjection;
using MinimalSdk.Models;
using MinimalSdk.Slices;

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

var displayDate = false;

app.MapGet("/displayDate", async (IDatastarServerSentEventService sse) =>
{
    displayDate = true;
    while (displayDate)
    {
        var today = DateTime.Now.ToString("%y-%M-%d %h:%m:%s");
        // TODO: use partial rather than explicit string
        await sse.MergeFragmentsAsync(
            $"""<div id="target"><span id="date"><b>{today}</b><button data-on-click="@get('/removeDate')">Remove</button></span></div>""");
        await Task.Delay(1000);
    }
});

app.MapGet("/removeDate", async (IDatastarServerSentEventService sse) =>
{
    displayDate = false;
    await sse.RemoveFragmentsAsync("#date");
});

app.MapPost("/changeOutput", async (IDatastarServerSentEventService sse, IDatastarSignalsReaderService dsSignals) =>
{
    var signals = await dsSignals.ReadSignalsAsync<HomeSignals>();
    var newSignals = new HomeSignals { Output = $"Your input: {signals.Input}" };
    await sse.MergeSignalsAsync(newSignals.Serialize());
});

Console.WriteLine($"RuntimeFeature.IsDynamicCodeSupported = {RuntimeFeature.IsDynamicCodeSupported}");
Console.WriteLine($"RuntimeFeature.IsDynamicCodeCompiled = {RuntimeFeature.IsDynamicCodeCompiled}");

app.Run();
