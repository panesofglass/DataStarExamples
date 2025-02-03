open System
open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Hosting
open MinimalApi

let args = Environment.GetCommandLineArgs()
let builder = WebApplication.CreateBuilder(args)

builder.Services.AddSingleton<NoteService>() |> ignore

let app = builder.Build()

// Configure browser refresh middleware in development
if app.Environment.IsDevelopment() then
    app.UseDeveloperExceptionPage() |> ignore

// Add after app builder but before other middleware
if not(app.Environment.IsDevelopment()) then
    app.UseExceptionHandler(fun errorApp ->
        errorApp.Run(fun context ->
            context.Response.StatusCode <- 500
            context.Response.ContentType <- "text/html"

            let errorHtml = """<div id="notes-list"><span class="note-item"><p>An error occurred. Please try again later.</p></span></div>"""

            Sse.sendServerSentEvent(context.Response, errorHtml, ValueOption.None, ValueOption.None, ValueOption.None, ValueOption.None, ValueOption.None)
        )
    ) |> ignore

// Add security headers
app.Use(fun (context: HttpContext) (next: Func<Task>) ->
    task {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff")
        context.Response.Headers.Append("X-Frame-Options", "DENY")
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block")
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin")
        context.Response.Headers.Append("Permissions-Policy",
            "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()")

        if not(app.Environment.IsDevelopment()) then
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains")

        do! next.Invoke()
    }
    :> Task
) |> ignore

let wwwRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot") 

app.UseDefaultFiles(DefaultFilesOptions(FileProvider = new PhysicalFileProvider(wwwRootPath))) |> ignore

app.UseStaticFiles(StaticFileOptions(FileProvider = new PhysicalFileProvider(wwwRootPath))) |> ignore

app.MapGet("/api/notes", NotesApi.get) |> ignore
app.MapPut("/api/search", NotesApi.put) |> ignore

app.Run()
