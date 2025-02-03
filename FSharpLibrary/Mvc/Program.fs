#nowarn "20"

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Mvc.Models
open Mvc.Services

let args = Environment.GetCommandLineArgs()
let builder = WebApplication.CreateBuilder(args)

builder
    .Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    |> ignore

builder.Services.AddSingleton<NoteService>() |> ignore

let app = builder.Build()

if not (builder.Environment.IsDevelopment()) then
    app.UseExceptionHandler("/Home/Error")
    app.UseHsts() |> ignore // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHttpsRedirection() |> ignore

app.UseStaticFiles() |> ignore
app.UseRouting() |> ignore
app.UseAuthorization() |> ignore

app.MapControllers() |> ignore
app.MapControllerRoute(
    name = "default",
    pattern = "{controller=Home}/{action=Index}/{id?}") |> ignore

app.Run()
