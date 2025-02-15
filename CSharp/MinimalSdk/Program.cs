using System.Runtime.CompilerServices;
using MinimalSdk.Endpoints;
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

app.MapHome();

app.MapDisplayDate();
app.MapChangeOutput();

app.MapQuiz();

Console.WriteLine(
    $"RuntimeFeature.IsDynamicCodeSupported = {RuntimeFeature.IsDynamicCodeSupported}"
);
Console.WriteLine($"RuntimeFeature.IsDynamicCodeCompiled = {RuntimeFeature.IsDynamicCodeCompiled}");

app.Run();
