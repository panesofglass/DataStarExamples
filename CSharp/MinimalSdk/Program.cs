using MinimalSdk.Slices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebEncoders();

var app = builder.Build();

app.MapGet("/", () => Results.Extensions.RazorSlice<Home, DateTime>(DateTime.Now));

app.Run();
